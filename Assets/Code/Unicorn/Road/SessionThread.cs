/********************************************************************
created:    2023-06-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Unicorn.IO;

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

                socket.Close();
            }
            catch (Exception ex)
            {
                Logo.Warn("Close socket by ex={0}", ex);
                Close();
            }
        }

        private Socket _ConnectSocket(IPAddress address, int port)
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
            //  Close socket by ex=System.Net.Sockets.SocketException (0x80004005): 由于套接字没有连接并且(当使用一个 sendto 调用发送数据报套接字时)没有提供地址，发送或接收数据的请求没有被接受。
            socket.Connect(address, port);

            _socket = socket;
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

        public void Send(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode)
        {
            errorCode = SocketError.NotConnected;
            if (_socket != null && !IsClosed())
            {
                _socket.Send(buffer, offset, size, socketFlags, out errorCode);
            }
        }

        public void ReceivePackets(List<Packet> packets)
        {
            lock (_locker)
            {
                if (_sharedPackets.Count > 0)
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

        public bool HasSocket()
        {
            return _socket != null;
        }

        public void Close()
        {
            Interlocked.Exchange(ref _isRunning, 0);
        }

        private readonly IPAddress _address;
        private readonly int _port;
        private Socket _socket;

        private long _isRunning = 1;
        private readonly List<Packet> _sharedPackets = new();
        private readonly object _locker = new();
    }
}