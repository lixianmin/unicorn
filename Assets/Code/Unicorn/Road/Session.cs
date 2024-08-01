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
    public class Session : Disposable
    {
        public void Connect(string hostNameOrAddress, int port, Func<Session, ISerde> serdeBuilder,
            Action<JsonHandshake> onHandShaken = null, Action onClosed = null)
        {
            Close();
            _reconnectAction = () =>
            {
                _serdeBuilder = serdeBuilder ?? throw new ArgumentNullException(nameof(serdeBuilder));
                _onHandShakenHandler = onHandShaken;
                _onClosedHandler = onClosed;

                try
                {
                    var addressList = Dns.GetHostAddresses(hostNameOrAddress);
                    if (!addressList.IsNullOrEmpty())
                    {
                        var address = addressList[0];
                        _sessionThread?.Close();
                        _sessionThread = new SessionThread(address, port);
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
            _sessionThread?.Close();
            _sessionThread = null;

            _serde = null;
            _serdeBuilder = null;

            _onHandShakenHandler = null;
            _onClosedHandler = null;
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

            _sessionThread?.Update();
        }

        private void _CheckReconnect()
        {
            if (_sessionThread != null && _sessionThread.IsClosed() && _reconnectAction != null &&
                Time.time > _nextReconnectTime)
            {
                // _nonce>0 意味着已经收到 HandShaken
                // server确保返回的 nonce>0
                if (_onClosedHandler != null && _nonce > 0)
                {
                    _onClosedHandler();
                    _onClosedHandler = null;
                    _nonce = 0;
                }

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

            _sessionThread?.Send(_heartbeatBuffer, 0, _heartbeatBuffer.Length);
        }

        private void _SendPacket(Packet pack)
        {
            if (_sessionThread != null)
            {
                var stream = _writer.BaseStream as OctetsStream;
                stream!.Reset();
                PacketTools.Encode(_writer, pack);

                var buffer = stream.GetBuffer();
                var size = (int)stream.Length;

                _sessionThread.Send(buffer, 0, size);
            }
        }

        private void _CheckReceivePackets()
        {
            if (_sessionThread != null)
            {
                _sessionThread.ReceivePackets(_packets);

                if (_packets.Count > 0)
                {
                    // _packets可以确认是主线程执行的, 但在遍历的过程中, 如果遇到了Kick,
                    // 则会调用Close, 则会把_packets清空
                    for (var i = 0; i < _packets.Count; i++)
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
                    var reason = Encoding.UTF8.GetString(pack.Data);
                    _Kick($"kicked by server, reason={reason}");
                    break;
                case PacketKind.RouteKind:
                    _OnReceivedRouteKind(pack);
                    break;
                case PacketKind.Echo:
                    _OnReceivedEcho(pack);
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

            // release的时候, 不打印routes细节
            if (os.IsReleaseMode)
            {
                Logo.Info($"handshake: nonce={handshake.nonce} heartbeat={handshake.heartbeat} gid={handshake.gid}");
            }
            else
            {
                Logo.Info("handshake={0}", text);
            }

            if (!_serverGid.IsNullOrEmpty() && _serverGid != handshake.gid)
            {
                _Kick("kicked by server restart");
                return;
            }

            _serverGid = handshake.gid;
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

            _nonce = handshake.nonce;
            _serde = _serdeBuilder(this);

            _HandshakeRe();
            _onHandShakenHandler?.Invoke(handshake);
        }

        private void _Kick(string reason)
        {
            Close();

            _serverGid = string.Empty;
            _onKickedHandler?.Invoke();
            Logo.Warn(reason);
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

            Logo.Info($"[_OnReceivedRouteKind()] kind={bean.kind} route={bean.route}");
        }

        private void _OnReceivedEcho(Packet pack)
        {
            _SendPacket(pack);
            Logo.Info($"[_OnReceivedEcho()] kind={pack.Kind} requestId={pack.RequestId}");
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
                if (_requestHandlers.Remove(requestId, out var handler))
                {
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
                _requestHandlers[requestId] = (data1, err) => { _CallHandler(handler, data1, err); };
            }

            _SendPacket(pack);
            return SocketError.Success;
        }

        public void On<T>(string route, Action<T, Error> handler) where T : new()
        {
            if (string.IsNullOrEmpty(route) || handler == null)
            {
                Logo.Warn($"invalid argument, route={route}, handler={handler}");
                return;
            }

            // _registeredHandlers的key必须使用string而不能使用byte[], 因为byte[]是一个ref类型, 没有像string一个重载GetHash()相关的方法
            _registeredHandlers[route] = (data1, err) => { _CallHandler(handler, data1, err); };
        }

        private void _CallHandler<T>(Action<T, Error> handler, byte[] data, Error err) where T : new()
        {
            if (_serde == null)
            {
                Logo.Warn($"_serde is null");
                return;
            }

            if (data != null)
            {
                var response = _serde.Deserialize<T>(data);
                try
                {
                    handler(response, null);
                }
                catch (Exception ex)
                {
                    Logo.Warn($"response={response}, ex={ex}");
                }
            }
            else
            {
                try
                {
                    handler(default, err);
                }
                catch (Exception ex)
                {
                    Logo.Warn($"err={err}, ex={ex}");
                }
            }
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
        private SessionThread _sessionThread;

        private ISerde _serde;
        private Func<Session, ISerde> _serdeBuilder;

        private Action<JsonHandshake> _onHandShakenHandler;
        private Action _onClosedHandler;
        private Action _onKickedHandler;
        private int _nonce;
        private string _serverGid = string.Empty;

        private float _nextHeartbeatTime = float.MaxValue;
        private float _heartbeatInterval;
        private byte[] _heartbeatBuffer;
        private int _requestIdGenerator;
    }
}