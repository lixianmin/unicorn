/********************************************************************
created:    2023-06-12
author:     lixianmin

现在的设计是有缺陷的, 除非改成_pendingStream在接收到服务器的确认协议后主动删除的方式,
而且还需要加上幂等, 也就是改成确定性消息, 否则一定会有丢失的风险

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Unicorn.IO;
using Unicorn.Collections;
namespace Unicorn.Road
{
    internal class SessionThread
    {
        public SessionThread(IPAddress address, int port)
        {
            _address = address;
            _port = port;

            var thread = new Thread(_Run);
            thread.Start();
        }

        private void _Run()
        {
            var socket = _ConnectSocket(_address, _port);
            if (socket == null)
            {
                Close();
                return;
            }

            _socket = socket;

            const int bufferSize = 4096;
            var buffer = new byte[bufferSize];
            var stream = new OctetsStream(128);
            var reader = new OctetsReader(stream);
            var packs = new List<Packet>();

            try
            {
                while (Interlocked.Read(ref _isRunning) == 1)
                {
                    var num = socket.Receive(buffer, 0, bufferSize, SocketFlags.None);
                    if (num > 0)
                    {
                        stream.Position = stream.Length; // 重置Position原因: Write()需要从尾部开始写
                        stream.Write(buffer, 0, num);
                        stream.Position = 0; // 重置Position原因: Read()需要从头部开始读
                        _OnReceivedData(reader, packs);
                        stream.Tidy();
                    }
                }

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                Close();
                Logo.Warn($"[_Run()] Close socket by ex={ex}");
            }
        }

        private static Socket _ConnectSocket(IPAddress address, int port)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // 只所以把连socket的操作放到Thread中, 是因为它会block, 会阻塞主线程做工作
            // 特别是当client切网, 需要重连的时候, 游戏会卡死, 玩家体验很差
            socket.Blocking = true;

            // _socket.ReceiveTimeout = 2000;   // 这两个Timeout的默认值都是0
            // _socket.SendTimeout = 1000;
            socket.SendBufferSize = 512 * 1024; // SendBufferSize的默认值是128 * 1024
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true); // Disable Nagle

            // 1. 目前测试得知, address使用127.0.0.1或正确的IP(如: 192.168.31.96)在macos上是可以正确连接到golang server的
            // 2. 使用ConnectAsync() 代替 Connect(), 是为了防止在connect的address错误的时候不卡死UI响应
            // 3. 但是, 使用ConnectAsync()时, 发现在肖老师的电脑上, 会反复报如下错误, 导致连不上
            //  Close socket by ex=System.Net.Sockets.SocketException (0x80004005): 由于套接字没有连接
            // 并且(当使用一个 sendto 调用发送数据报套接字时)没有提供地址，发送或接收数据的请求没有被接受。

            try
            {
                socket.Connect(address, port);
            }
            catch (Exception ex)
            {
                Logo.Warn($"ex={ex}");
                return null;
            }

            // Logo.Warn($"_socket.SendBufferSize={_socket.SendBufferSize}, receiveTimeout = {_socket.ReceiveTimeout}, sendTime={_socket.SendTimeout}");

            return socket;
        }

        private void _OnReceivedData(OctetsReader reader, List<Packet> packs)
        {
            PacketTools.Decode(reader, packs);
            if (packs.Count > 0)
            {
                lock (_locker)
                {
                    _sharedPackets.AddRange(packs);
                }

                packs.Clear();
            }
        }

        internal void Send(byte[] buffer, int offset, int size)
        {
            if (buffer.IsNullOrEmpty() || offset < 0 || size <= 0)
            {
                return;
            }

            // _pendingStream不需要lock, 它用于接收数据并转交给_sendingStream, 因此_pendingStream所有相关操作都是主线程
            _pendingStream.Write(buffer, offset, size);

            // 立即调用, 尽可能减少发送延迟
            _CheckSendPendingStream();
        }

        internal void Update()
        {
            // 之所以设计成先缓存到_pendingStream中的方案, 因为据说socket.BeginSend()在callback回来之前不能反复调用
            // 参考: https://stackoverflow.com/questions/21284000/predictable-behaviour-of-overlapping-socket-beginsend-endsend-calls
            _CheckSendPendingStream();
        }

        private void _CheckSendPendingStream()
        {
            var socket = _socket;
            if (socket != null && _pendingStream.Length > 0 && Interlocked.Read(ref _canSend) == 1)
            {
                Interlocked.Exchange(ref _canSend, 0);

                // _sendingStream也不需要lock, 因为它的数据虽然跨线程, 但是对它的修改全是在主线程里进行的, 而且在
                // _OnSendCallback()回调之前, 不会再次修改
                _sendingStream.Tidy();
                _pendingStream.WriteTo(_sendingStream);
                _pendingStream.Tidy();

                try
                {
                    var buffer = _sendingStream.GetBuffer();
                    var size = (int)_sendingStream.Length;
                    socket.BeginSend(buffer, 0, size, SocketFlags.None, _OnSendCallback, null);
                }
                catch (Exception ex)
                {
                    Close();
                    Logo.Warn($"[Update()] Close socket by ex={ex}");
                }
            }
        }

        /// <summary>
        /// 这个回调方法跟BeginSend()不是一个线程
        /// </summary>
        /// <param name="ar"></param>
        private void _OnSendCallback(IAsyncResult ar)
        {
            var socket = _socket;
            if (socket != null && !IsClosed())
            {
                try
                {
                    // EndSend()是一个blocking操作
                    socket.EndSend(ar);
                }
                catch (Exception ex)
                {
                    Close();
                    Logo.Warn($"[_OnSendCallback()] Close socket by ex={ex}");
                }
            }

            Interlocked.Exchange(ref _canSend, 1);
        }

        internal void ReceivePackets(Slice<Packet> packets)
        {
            lock (_locker)
            {
                if (_sharedPackets.Size > 0)
                {
                    packets.AddRange(_sharedPackets);
                    _sharedPackets.Clear();
                }
            }
        }

        public bool IsClosed()
        {
            return Interlocked.Read(ref _isRunning) == 0;
        }

        public void Close()
        {
            Interlocked.Exchange(ref _isRunning, 0);
        }

        private readonly IPAddress _address;
        private readonly int _port;
        private Socket _socket;

        private long _canSend = 1;
        private readonly OctetsStream _pendingStream = new();
        private readonly OctetsStream _sendingStream = new();

        private long _isRunning = 1;
        private readonly Slice<Packet> _sharedPackets = new();
        private readonly object _locker = new();
    }
}