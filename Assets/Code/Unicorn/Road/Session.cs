/********************************************************************
created:    2023-06-10
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Unicorn.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Unicorn.Road
{
    public partial class Session : Disposable
    {
        public void Connect(string hostNameOrAddress, int port, ISerde serde, Action<JsonHandshake> onHandShaken = null)
        {
            Close();
            _reconnectAction = () =>
            {
                _serde = serde ?? throw new ArgumentNullException(nameof(serde));
                _onHandShaken = onHandShaken;

                try
                {
                    var addressList = Dns.GetHostAddresses(hostNameOrAddress);
                    if (!addressList.IsNullOrEmpty())
                    {
                        var address = addressList[0];
                        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        _socket.Blocking = true;
                        // _socket.ReceiveTimeout = 2000;   // 这两个Timeout的默认值都是0
                        // _socket.SendTimeout = 1000;
                        _socket.SendBufferSize = 512 * 1024; // SendBufferSize的默认值是128 * 1024
                        _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true); // Disable Nagle

                        // 1. 目前测试得知, address使用127.0.0.1或正确的IP(如: 192.168.31.96)在macos上是可以正确连接到golang server的
                        // 2. 使用ConnectAsync() 代替 Connect(), 是为了防止在connect的address错误的时候不卡死UI响应
                        // 3. 但是, 使用ConnectAsync()时, 发现在肖老师的电脑上, 会反复报如下错误, 导致连不上
                        //  Close socket by ex=System.Net.Sockets.SocketException (0x80004005): 由于套接字没有连接并且(当使用一个 sendto 调用发送数据报套接字时)没有提供地址，发送或接收数据的请求没有被接受。
                        _socket.Connect(address, port);
                        // Logo.Warn($"_socket.SendBufferSize={_socket.SendBufferSize}, receiveTimeout = {_socket.ReceiveTimeout}, sendTime={_socket.SendTimeout}");

                        _receiverThread = new ReceiverThread(_socket);
                    }
                }
                catch (Exception ex)
                {
                    Logo.Warn($"ex={ex}");
                }
            };

            _reconnectAction();
        }

        protected override void _DoDispose(int flags)
        {
            Close();
        }

        public void Close()
        {
            _socket?.Close();
            _socket = null;

            _receiverThread?.Close();
            _receiverThread = null;

            _serde = null;
            _onHandShaken = null;
            _nonce = 0;

            _nextHeartbeatTime = float.MaxValue;
            _packets.Clear();
            _requestHandlers.Clear();
        }

        public void Update()
        {
            _CheckReconnect();
            _CheckReceivePackets();
            _CheckSendHeartbeat();
        }

        private void _CheckReconnect()
        {
            if (_receiverThread != null && _receiverThread.IsClosed() && _reconnectAction != null &&
                Time.time > _nextReconnectTime)
            {
                // 这一句必须在前面, 因为后面的_reconnectAction()可能会异常
                var randomSeconds = Random.Range(2, 5);
                _nextReconnectTime = Time.time + randomSeconds;
                _reconnectAction();
                // Logo.Info($"time={Time.time}, _nextReconnectTime={_nextReconnectTime}");
            }
        }

        private void _CheckSendHeartbeat()
        {
            if (Time.time > _nextHeartbeatTime)
            {
                _SendHeartbeat();
                _nextHeartbeatTime = Time.time + _heartbeatInterval;
            }
        }

        private void _SendHeartbeat()
        {
            if (_heartbeatBuffer == null)
            {
                var pack = new Packet
                {
                    Kind = (int)PacketKind.Heartbeat
                };

                var stream = _writer.BaseStream as OctetsStream;
                stream!.Reset();
                PacketTools.Encode(_writer, pack);

                var buffer = stream.GetBuffer();
                var size = (int)stream.Length;

                _heartbeatBuffer = new byte[size];
                Buffer.BlockCopy(buffer, 0, _heartbeatBuffer, 0, size);
            }

            if (null != _socket)
            {
                _socket.Send(_heartbeatBuffer, 0, _heartbeatBuffer.Length, SocketFlags.None, out var err);
                if (err != SocketError.Success)
                {
                    Logo.Warn($"[_SendHeartbeat()] err={err}");
                    _receiverThread?.Close();
                }
            }
        }

        private SocketError _SendPacket(Packet pack)
        {
            if (_socket != null)
            {
                var stream = _writer.BaseStream as OctetsStream;
                stream!.Reset();
                PacketTools.Encode(_writer, pack);

                var buffer = stream.GetBuffer();
                var size = (int)stream.Length;
                _socket.Send(buffer, 0, size, SocketFlags.None, out var err);
                return err;
            }

            return SocketError.NotConnected;
        }

        private void _CheckReceivePackets()
        {
            if (_receiverThread != null)
            {
                _receiverThread.ReceivePackets(_packets);

                var size = _packets.Count;
                if (size > 0)
                {
                    for (int i = 0; i < size; i++)
                    {
                        var pack = _packets[i];
                        _OnReceivedPacket(pack);
                    }

                    _packets.Clear();
                }
            }
        }

        private void _OnReceivedPacket(Packet pack)
        {
            switch ((PacketKind)pack.Kind)
            {
                case PacketKind.Handshake:
                    _OnReceivedHandshake(pack);
                    break;
                case PacketKind.Heartbeat:
                    // Logo.Info(pack);
                    break;
                case PacketKind.Kick:
                    // _socket.close(0, 'kicked')
                    Close();
                    _onKickedHandler?.Invoke();
                    Logo.Warn("kicked by server");
                    break;
                case PacketKind.RouteKind:
                    _OnReceivedRouteKind(pack);
                    break;
                default:
                    _OnReceivedUserdata(pack);
                    break;
            }
        }

        private void _OnReceivedHandshake(Packet pack)
        {
            var text = Encoding.UTF8.GetString(pack.Data);
            var handshake = (JsonHandshake)JsonUtility.FromJson(text, typeof(JsonHandshake));
            Logo.Info("handshake={0}", text);

            _heartbeatInterval = handshake.heartbeat;
            _nextHeartbeatTime = Time.time + _heartbeatInterval;

            _routeKinds.Clear();
            _kindRoutes.Clear();
            var routes = handshake.routes ?? Array.Empty<string>();
            var size = routes.Length;
            for (var i = 0; i < size; i++)
            {
                var kind = (int)PacketKind.UserBase + i;
                var route = routes[i];
                _routeKinds[route] = kind;
                _kindRoutes[kind] = route;
            }

            _HandshakeRe();
            _nonce = handshake.nonce;
            _onHandShaken?.Invoke(handshake);
        }

        private void _HandshakeRe()
        {
            var reply = new JsonHandshakeRe
            {
                serde = _serde.GetName()
            };

            var jsonSerde = new JsonSerde();
            var replyData = jsonSerde.Serialize(reply);
            var pack = new Packet
            {
                Kind = (int)PacketKind.HandshakeRe,
                Data = replyData
            };

            _SendPacket(pack);
        }

        private void _OnReceivedRouteKind(Packet pack)
        {
            var text = Encoding.UTF8.GetString(pack.Data);
            var bean = (JsonRouteKind)JsonUtility.FromJson(text, typeof(JsonRouteKind));
            _routeKinds[bean.route] = bean.kind;
            _kindRoutes[bean.kind] = bean.route;

            Logo.Info($"[_OnReceivedRouteKind()], kind={bean.kind}, route={bean.route}");
        }

        private void _OnReceivedUserdata(Packet pack)
        {
            var kind = pack.Kind;
            if (kind < (int)PacketKind.UserBase)
            {
                Logo.Warn($"[_OnReceivedUserdata()] invalid kind={kind}");
                return;
            }

            var handler = _FetchHandler(pack);
            if (null == handler)
            {
                // 有些协议, 真不想处理, 就不设置handlers了. 通常只要有requestId, 就是故意不处理的
                if (pack.RequestId == 0)
                {
                    var message = $"no handler, kind={kind}, requestId={pack.RequestId}";
                    Logo.Warn(message);
                }

                return;
            }

            var hasError = pack.Code is { Length: > 0 };
            if (hasError)
            {
                var err = new Error()
                {
                    Code = Encoding.UTF8.GetString(pack.Code),
                    Message = Encoding.UTF8.GetString(pack.Data)
                };
                handler(null, err);
            }
            else
            {
                handler(pack.Data, null);
            }
        }

        private Action<byte[], Error> _FetchHandler(Packet pack)
        {
            var requestId = pack.RequestId;
            if (requestId != 0)
            {
                if (_requestHandlers.TryGetValue(requestId, out var handler))
                {
                    _requestHandlers.Remove(requestId);
                    return handler;
                }
            }
            else
            {
                var route = _kindRoutes[pack.Kind];
                _registeredHandlers.TryGetValue(route, out var handler);
                return handler;
            }

            return null;
        }

        public SocketError Request<T>(string route, object request, Action<T, Error> handler) where T : new()
        {
            if (_serde == null)
            {
                return SocketError.Shutdown;
            }

            if (string.IsNullOrEmpty(route) || null == request)
            {
                return SocketError.InvalidArgument;
            }

            var data = _serde.Serialize(request);
            var requestId = ++_requestIdGenerator;
            var has = _routeKinds.TryGetValue(route, out var kind);
            if (!has)
            {
                // 由抛异常改为打日志, 这是为了兼容某些新的client与旧的server
                Logo.Warn($"invalid route={route}");
                return SocketError.InvalidArgument;
            }

            var pack = new Packet
            {
                Kind = kind,
                RequestId = requestId,
                Data = data
            };

            // if (!has)
            // {
            //     var routeData = Encoding.UTF8.GetBytes(route);
            //     pack.Kind = (int)PacketKind.RouteBase + routeData.Length;
            //     pack.Route = routeData;
            // }

            if (handler != null)
            {
                _requestHandlers[requestId] = (data1, err) =>
                {
                    if (data1 != null)
                    {
                        var response = _serde.Deserialize<T>(data1);
                        handler(response, null);
                    }
                    else
                    {
                        handler(default, err);
                    }
                };
            }

            return _SendPacket(pack);
        }

        public void On<T>(string route, Action<T, Error> handler) where T : new()
        {
            if (string.IsNullOrEmpty(route) || handler == null)
            {
                Logo.Warn($"invalid argument, route={route}, handler={handler}");
                return;
            }

            // _registeredHandlers的key必须使用string而不能使用byte[], 因为byte[]是一个ref类型, 没有像string一个重载GetHash()相关的方法
            _registeredHandlers[route] = (data1, err) =>
            {
                if (data1 != null)
                {
                    var response = _serde.Deserialize<T>(data1);
                    handler(response, null);
                }
                else
                {
                    handler(default, err);
                }
            };
        }

        public void OnKicked(Action handler)
        {
            _onKickedHandler = handler;
        }

        public int GetNonce()
        {
            return _nonce;
        }

        private readonly OctetsWriter _writer = new(new OctetsStream());
        private readonly List<Packet> _packets = new();
        private readonly Dictionary<string, int> _routeKinds = new();
        private readonly Dictionary<int, string> _kindRoutes = new();
        private readonly Dictionary<int, Action<byte[], Error>> _requestHandlers = new();
        private readonly Dictionary<string, Action<byte[], Error>> _registeredHandlers = new();

        private Action _reconnectAction;
        private float _nextReconnectTime;
        private Socket _socket;
        private ReceiverThread _receiverThread;

        private ISerde _serde;
        private Action<JsonHandshake> _onHandShaken;
        private Action _onKickedHandler;
        private int _nonce;

        private float _nextHeartbeatTime = float.MaxValue;
        private float _heartbeatInterval;
        private byte[] _heartbeatBuffer;
        private int _requestIdGenerator;
    }
}