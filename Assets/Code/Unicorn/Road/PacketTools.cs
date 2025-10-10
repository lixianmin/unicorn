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
using Unicorn.IO;

namespace Unicorn.Road
{
    public static class PacketTools
    {
        public static void Encode(OctetsWriter writer, Packet pack)
        {
            if (writer != null)
            {
                writer.Write7BitEncodedInt(pack.Kind);
                // pack.kind是kind还是RouteBase+len(route), 这是在调用EncodePacket之前就准备好的
                // route需要是一个[]byte而不能是string, 因为需要在外围计算route的长度, 并赋值到kind, 计算前我们就已经拿到[]byte了
                // if (pack.Kind > (int)PacketKind.RouteBase)
                // {
                //     writer.BaseStream.Write(pack.Route, 0, pack.Route.Length);
                // }

                writer.Write7BitEncodedInt(pack.RequestId);
                _WriteBytes(writer, pack.Code);
                _WriteBytes(writer, pack.Data);
            }
        }

        public static void Decode(OctetsReader reader, IList<Packet> packets)
        {
            if (reader == null || packets == null)
            {
                return;
            }

            var stream = reader.BaseStream;
            long lastPosition = 0;

            try
            {
                while (true)
                {
                    lastPosition = stream.Position;
                    var kind = reader.Read7BitEncodedInt();

                    // byte[] route = null;
                    // if (kind > (int)PacketKind.RouteBase)
                    // {
                    //     var size = kind - (int)PacketKind.RouteBase;
                    //     route = new byte[size];
                    //     var readSize = stream.Read(route, 0, size);
                    //     if (readSize != size)
                    //     {
                    //         throw new EndOfStreamException(nameof(size));
                    //     }
                    // }

                    var requestId = reader.Read7BitEncodedInt();
                    var code = _ReadBytes(reader);
                    var data = _ReadBytes(reader);

                    var pack = new Packet { Kind = kind, RequestId = requestId, Code = code, Data = data };
                    packets.Add(pack);
                }
            }
            catch (Exception)
            {
                stream.Seek(lastPosition, SeekOrigin.Begin);
            }
        }

        private static Chunk<byte> _ReadBytes(OctetsReader reader)
        {
            var size = reader.Read7BitEncodedInt();
            var bytes = reader.ReadBytes(size);
            if (bytes.Length != size)
            {
                throw new EndOfStreamException(nameof(size));
            }

            return new Chunk<byte>
            {
                Items = bytes,
                Size = size
            };
        }

        private static void _WriteBytes(OctetsWriter writer, Chunk<byte> chunk)
        {
            if (chunk.Size > 0)
            {
                var size = chunk.Size;
                writer.Write7BitEncodedInt(size);
                writer.Write(chunk.Items, 0, size);
            }
            else
            {
                writer.Write7BitEncodedInt(0);
            }
        }
    }
}