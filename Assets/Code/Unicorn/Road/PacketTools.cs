// /********************************************************************
// created:    2023-06-10
// author:     lixianmin
//
// Copyright (C) - All Rights Reserved
// *********************************************************************/
//
// using System;
// using System.Collections.Generic;
// using System.IO;
// using Unicorn.IO;
//
// namespace Unicorn.Road
// {
//     public static class PacketTools
//     {
//         public static void Encode(OctetsWriter writer, Packet pack)
//         {
//             if (writer != null)
//             {
//                 writer.Write7BitEncodedInt(pack.Kind);
//                 // pack.kind是kind还是RouteBase+len(route), 这是在调用EncodePacket之前就准备好的
//                 // route需要是一个[]byte而不能是string, 因为需要在外围计算route的长度, 并赋值到kind, 计算前我们就已经拿到[]byte了
//                 // if (pack.Kind > (int)PacketKind.RouteBase)
//                 // {
//                 //     writer.BaseStream.Write(pack.Route, 0, pack.Route.Length);
//                 // }
//
//                 writer.Write7BitEncodedInt(pack.RequestId);
//                 _WriteBytes(writer, pack.Code);
//                 _WriteBytes(writer, pack.Data);
//             }
//         }
//
//         public static void Decode(OctetsReader reader, IList<Packet> packets)
//         {
//             if (reader == null || packets == null)
//             {
//                 return;
//             }
//
//             var stream = reader.BaseStream;
//             long lastPosition = 0;
//
//             try
//             {
//                 while (true)
//                 {
//                     lastPosition = stream.Position;
//                     var kind = reader.Read7BitEncodedInt();
//
//                     // byte[] route = null;
//                     // if (kind > (int)PacketKind.RouteBase)
//                     // {
//                     //     var size = kind - (int)PacketKind.RouteBase;
//                     //     route = new byte[size];
//                     //     var readSize = stream.Read(route, 0, size);
//                     //     if (readSize != size)
//                     //     {
//                     //         throw new EndOfStreamException(nameof(size));
//                     //     }
//                     // }
//
//                     var requestId = reader.Read7BitEncodedInt();
//                     var code = _ReadBytes(reader);
//                     var data = _ReadBytes(reader);
//
//                     var pack = new Packet { Kind = kind, RequestId = requestId, Code = code, Data = data };
//                     packets.Add(pack);
//                 }
//             }
//             catch (Exception)
//             {
//                 stream.Seek(lastPosition, SeekOrigin.Begin);
//             }
//         }
//
//         private static byte[] _ReadBytes(OctetsReader reader)
//         {
//             var size = reader.Read7BitEncodedInt();
//             var bytes = reader.ReadBytes(size);
//             if (bytes.Length != size)
//             {
//                 throw new EndOfStreamException(nameof(size));
//             }
//
//             return bytes;
//         }
//
//         private static void _WriteBytes(OctetsWriter writer, byte[] bytes)
//         {
//             if (bytes != null)
//             {
//                 var size = bytes.Length;
//                 writer.Write7BitEncodedInt(size);
//                 writer.Write(bytes);
//             }
//             else
//             {
//                 writer.Write7BitEncodedInt(0);
//             }
//         }
//     }
// }