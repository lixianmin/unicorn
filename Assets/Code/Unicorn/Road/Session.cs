/********************************************************************
created:    2023-06-10
author:     lixianmin

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Unicorn.IO;
using Unicorn.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Unicorn.Road
{
    public class Session : Disposable
    {
        private class Handler
        {
            public void Invoke(Chunk<byte> data, Error err)
            {
                done = true;
                callback(data, err);
            }

            public float expireTime;
            public Action<Chunk<byte>, Error> callback;
            public bool done;
        }

        public Session()
        {
            AtDisposing += Close;
        }

        public void Connect(string hostNameOrAddress, int port, Func<Session, ISerde> serdeBuilder,
            Action<JsonHandshake> onHandShaken = null, Action onClosed = null)
        {
            Close();

            var addressList = _GetAddresses(hostNameOrAddress);
            if (addressList.IsNullOrEmpty())
            {
                Logo.Warn($"empty addressList, please check the network, hostNameOrAddress={hostNameOrAddress}");
                return;
            }

            _reconnectAction = () =>
            {
                _serdeBuilder = serdeBuilder ?? throw new ArgumentNullException(nameof(serdeBuilder));
                _onHandShakenHandler = onHandShaken;
                _onClosedHandler = onClosed;

                try
                {
                    var index = Random.Range(0, addressList.Length);
                    var address = addressList[index];
                    _sessionThread?.Close();
                    _sessionThread = new SessionThread(address, port);
                }
                catch (Exception ex)
                {
                    Logo.Warn($"ex={ex}");
                }
            };

            _reconnectAction();
        }

        private static IPAddress[] _GetAddresses(string hostNameOrAddress)
        {
            try
            {
                const int maxRetry = 3;
                for (var i = 0; i < maxRetry; i++)
                {
                    var addressList = Dns.GetHostAddresses(hostNameOrAddress);
                    if (!addressList.IsNullOrEmpty())
                    {
                        return addressList;
                    }
                }
            }
            catch (Exception ex)
            {
                Logo.Warn($"ex={ex}");
            }

            return null;
        }

        public void Close()
        {
            // 断线重连时, 强制所有的handler超时, 防止handler回调不到导致tree被永远卡死
            // 这个放到前面, 因为会使用到_serde变量
            if (_requestHandlers.Count > 0)
            {
                foreach (var handler in _requestHandlers.Values)
                {
                    handler.Invoke(default, _clientSideTimeoutError);
                }

                _requestHandlers.Clear();
            }

            _sessionThread?.Close();
            _sessionThread = null;

            _serde = null;
            _serdeBuilder = null;

            _onHandShakenHandler = null;
            _onClosedHandler = null;
            // _onKickedHandler = null; // 在Connect()的时候, 会调用Close()一次, 因此不能在这里重置_onKickedHandler, 否则就没了
            _nonce = 0;

            _nextHeartbeatTime = float.MaxValue;
            _packets.Clear();

            // 1. 2025-07-02 有一个改动, 是将NetManager中的_session重新new了一个, 理由是"主动退出时防止断线重连", 但这个改动导致游戏初始
            //    化时, 通过session.On(route, xxx)注册的回调都找不到了, 因此不能重建session, 全局只能使用一个
            // 2. 2025-07-16 尝试在session.Close()的时候, 默认就直接关闭 _reconnectionAction. 为了防止在断线重连的过程中出问题, 断线
            //    重连的逻辑中会先备份一下_reconnectAction变量
            _reconnectAction = null;
        }

        public void Update()
        {
            _CheckReconnect();
            _CheckReceivePackets();
            _CheckSendHeartbeat();
            _CheckRequestTimeout();

            _sessionThread?.Update();
        }

        private void _CheckReconnect()
        {
            if (_reconnectAction == null)
            {
                return;
            }

            var now = Time.time;
            if (_sessionThread == null || _sessionThread.IsClosed() || now > _heartbeatTimeoutTime)
            {
                // _nonce>0 意味着已经收到 HandShaken
                // server确保返回的 nonce>0
                var reconnectAction = _reconnectAction;
                if (_onClosedHandler != null && _nonce > 0)
                {
                    CallbackTools.Handle(ref _onClosedHandler, "");
                    _nonce = 0;

                    // 2025-07-16 目前Close()方法会重置_reconnectAction的值, 但如果这个是由于断线重连的逻辑调用了Close(), 则先备份一下, 不能因此进行不下去
                    _reconnectAction = reconnectAction;
                }

                if (now >= _nextReconnectTime)
                {
                    var backoffSeconds = Math.Min(1 * (float)Math.Pow(2, _reconnectAttempt), 30);
                    _nextReconnectTime = now + backoffSeconds;
                    _reconnectAttempt++;

                    _reconnectAction();
                }
            }
            else
            {
                _reconnectAttempt = 0;
                _nextReconnectTime = 0;
            }
        }

        private void _CheckSendHeartbeat()
        {
            var time = Time.time;
            if (time > _nextHeartbeatTime)
            {
                _SendHeartbeat();
                _nextHeartbeatTime = time + _heartbeatInterval;
            }
        }

        private void _CheckRequestTimeout()
        {
            var now = Time.time;
            if (now > _nextCheckRequestTimeoutTime && _requestHandlers.Count > 0)
            {
                foreach (var handler in _requestHandlers.Values)
                {
                    if (now > handler.expireTime)
                    {
                        handler.Invoke(default, _clientSideTimeoutError);
                    }
                }

                _requestHandlers.RemoveAll((_, handler) => handler.done);
                _nextCheckRequestTimeoutTime = now + 0.5f;
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

                if (_packets.Size > 0)
                {
                    // _packets可以确认是主线程执行的, 但在遍历的过程中, 如果遇到了Kick,
                    // 则会调用Close, 则会把_packets清空
                    for (var i = 0; i < _packets.Size; i++)
                    {
                        var pack = _packets.Items[i];
                        _OnReceivedPacket(pack);
                        pack.ReturnToPool();
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
                    const float timeout = 5f;
                    _heartbeatTimeoutTime = Time.time + timeout;
                    // Logo.Info("[_OnReceivedPacket] received heartbeat");
                    break;
                case PacketKind.Kick:
                    var reason = _EncodeToString(pack.Data);
                    _Kick(reason);
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
            var text = _EncodeToString(pack.Data);
            var handshake = (JsonHandshake)JsonUtility.FromJson(text, typeof(JsonHandshake));

            Logo.Info($"handshake: nonce={handshake.nonce} heartbeat={handshake.heartbeat} sid={handshake.sid}");
            // if (!_serverGid.IsNullOrEmpty() && _serverGid != handshake.gid)
            // {
            //     _Kick("kicked by server restart");
            //     return;
            // }

            // _serverGid = handshake.gid;
            _heartbeatInterval = handshake.heartbeat;
            _nextHeartbeatTime = Time.time + _heartbeatInterval;

            _routeKinds.Clear();
            _kindRoutes.Clear();

            // 解码handshake.routes: base64解码 -> deflate-raw解压缩 -> 按空格分割
            var base64Data = Convert.FromBase64String(handshake.routes);
            using (var inputStream = new MemoryStream(base64Data))
            using (var deflateStream = new DeflateStream(inputStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(deflateStream, Encoding.UTF8))
            {
                var decompressedData = reader.ReadToEnd();
                var routes = decompressedData.Split(' ');
                var size = routes.Length;
                for (var i = 0; i < size; i++)
                {
                    var kind = (int)PacketKind.UserBase + i;
                    var route = routes[i];
                    _routeKinds[route] = kind;
                    _kindRoutes[kind] = route;
                }

                // release的时候, 不打印routes细节
                if (!os.IsReleaseMode)
                {
                    Logo.Info("handshake: routes={0}", decompressedData);
                }
            }

            _nonce = handshake.nonce;
            _serde = _serdeBuilder(this);

            _HandshakeRe();
            _onHandShakenHandler?.Invoke(handshake);
        }

        private void _Kick(string reason)
        {
            Close();

            // if (reason == "GlobalRateLimit")
            // {
            //     _nextReconnectTime = Time.time + 10f;
            // }

            // ~~如果被踢了, 就老老实实的退出去吧, 别想着断线重连了~~
            // _reconnectAction = null;

            // _serverGid = string.Empty;
            CallbackTools.Handle(_onKickedHandler, reason, string.Empty);
            Logo.Warn($"kicked by server, reason={reason}");
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
            var text = _EncodeToString(pack.Data);
            var bean = (JsonRouteKind)JsonUtility.FromJson(text, typeof(JsonRouteKind));
            _routeKinds[bean.route] = bean.kind;
            _kindRoutes[bean.kind] = bean.route;

            Logo.Info($"[_OnReceivedRouteKind()] kind={bean.kind} route={bean.route}");
        }

        private static string _EncodeToString(Chunk<byte> chunk)
        {
            return Encoding.UTF8.GetString(chunk.Items.AsSpan(0, chunk.Size));
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
            if (handler == null)
            {
                // 有些协议, 真不想处理, 就不设置handlers了. 通常只要有requestId, 就是故意不处理的
                if (pack.RequestId == 0)
                {
                    var message = $"no handler, kind={kind}, requestId={pack.RequestId}";
                    Logo.Warn(message);
                }

                return;
            }

            var hasError = pack.Code.Items is { Length: > 0 };
            if (hasError)
            {
                var err = new Error
                {
                    Code = _EncodeToString(pack.Code),
                    Message = _EncodeToString(pack.Data)
                };
                handler.Invoke(default, err);
            }
            else
            {
                handler.Invoke(pack.Data, null);
            }
        }

        private Handler _FetchHandler(Packet pack)
        {
            var requestId = pack.RequestId;
            if (requestId != 0)
            {
                _requestHandlers.TryGetValue(requestId, out var handler);
                return handler;
            }
            else
            {
                var route = _kindRoutes[pack.Kind];
                _registeredHandlers.TryGetValue(route, out var item);
                return item as Handler;
            }
        }

        // public SocketError Request<T>(string route, object request, Action<T, Error> handler) where T : new()
        // {
        //     const float timeout = 3600;
        //     return Request(route, request, timeout, handler);
        // }

        public SocketError Request<T>(string route, object request, float timeout, Action<T, Error> handler)
            where T : new()
        {
            if (_serde == null)
            {
                return SocketError.Shutdown;
            }

            // 发送handshake之后, 远程server才建立_serde, 才能发数据到远程
            var isHandshakeSent = _nonce != 0;
            if (!isHandshakeSent)
            {
                return SocketError.NotConnected;
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
                // 这是, 通过创建匿名函数, 把handler的参数的类型信息T给隐藏了
                _requestHandlers[requestId] = new Handler
                {
                    expireTime = Time.time + (timeout > 0 ? timeout : 0),
                    callback = (data1, err1) => { _CallHandler(handler, data1, err1); }
                };
            }

            _SendPacket(pack);
            pack.ReturnToPool();
            return SocketError.Success;
        }

        public void On<T>(string route, Action<T, Error> handler) where T : new()
        {
            if (string.IsNullOrEmpty(route) || handler == null)
            {
                Logo.Warn($"invalid argument, route={route}, handler={handler}");
                return;
            }

            // 1. _registeredHandlers的key必须使用string而不能使用byte[], 因为byte[]是一个ref类型, 没有像string一个重载GetHash()相关的方法
            // 2. route对应的Handler, 这个是游戏一开始初始化的时候注册的, 要小心切换session实例就找不到了
            _registeredHandlers[route] = new Handler
            {
                callback = (data1, err) => { _CallHandler(handler, data1, err); }
            };
        }

        private void _CallHandler<T>(Action<T, Error> handler, Chunk<byte> data, Error err) where T : new()
        {
            if (_serde == null)
            {
                Logo.Warn("_serde is null");
                return;
            }

            // data.Size有时候正常情况下也是0
            if (err == null)
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

        public void OnKicked(Action<string> handler)
        {
            _onKickedHandler = handler;
        }

        public int GetNonce()
        {
            return _nonce;
        }

        private readonly OctetsWriter _writer = new(new OctetsStream());
        private readonly Slice<Packet> _packets = new();
        private readonly Dictionary<string, int> _routeKinds = new();
        private readonly Dictionary<int, string> _kindRoutes = new();
        private readonly SortedTable<int, Handler> _requestHandlers = new();
        private readonly Dictionary<string, object> _registeredHandlers = new();

        private Action _reconnectAction;
        private int _reconnectAttempt;
        private float _nextReconnectTime;
        private SessionThread _sessionThread;

        private ISerde _serde;
        private Func<Session, ISerde> _serdeBuilder;

        private Action<JsonHandshake> _onHandShakenHandler;
        private Action _onClosedHandler;
        private Action<string> _onKickedHandler;
        private int _nonce;
        // private string _serverGid = string.Empty;

        private float _nextHeartbeatTime = float.MaxValue;
        private float _heartbeatInterval;
        private float _heartbeatTimeoutTime = float.MaxValue;
        private float _nextCheckRequestTimeoutTime;

        private readonly Error _clientSideTimeoutError = new()
            { Code = "ClientSideTimeout", Message = "request timeout on client side" };

        private byte[] _heartbeatBuffer;
        private int _requestIdGenerator;
    }
}