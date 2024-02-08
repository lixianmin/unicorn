/********************************************************************
created:    2023-06-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Unicorn.IO;

namespace Unicorn.Road
{
    partial class Session
    {
        class ReceiverThread
        {
            public ReceiverThread(Socket socket)
            {
                _socket = socket;
                var thread = new Thread(_Run);
                thread.Start();
            }

            private void _Run()
            {
                const int bufferSize = 4096;
                var buffer = new byte[bufferSize];
                var stream = new OctetsStream(128);
                var reader = new OctetsReader(stream);
                var packs = new List<Packet>();

                try
                {
                    while (Interlocked.Read(ref _isRunning) == 1)
                    {
                        var num = _socket.Receive(buffer, 0, bufferSize, SocketFlags.None);
                        if (num > 0)
                        {
                            stream.Position = stream.Length; // 重置Position原因: Write()需要从尾部开始写
                            stream.Write(buffer, 0, num);
                            stream.Position = 0; // 重置Position原因: Read()需要从头部开始读
                            _OnReceivedData(reader, packs);
                            stream.Tidy();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logo.Warn("Close socket by ex={0}", ex);
                    Close();
                }
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

            public void Close()
            {
                Interlocked.Exchange(ref _isRunning, 0);
            }

            private readonly Socket _socket;
            private long _isRunning = 1;
            private readonly List<Packet> _sharedPackets = new();
            private readonly object _locker = new();
        }
    }
}