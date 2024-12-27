// /********************************************************************
// created:    2023-06-12
// author:     lixianmin
//
// Copyright (C) - All Rights Reserved
// *********************************************************************/
//
// using System;
// using System.Text;
// using UnityEngine;
//
// namespace Unicorn.Road
// {
//     public class JsonSerde : ISerde
//     {
//         public byte[] Serialize(object item)
//         {
//             if (null != item)
//             {
//                 if (item is byte[] buffer)
//                 {
//                     return buffer;
//                 }
//
//                 var text = JsonUtility.ToJson(item);
//                 var bytes = Encoding.UTF8.GetBytes(text);
//                 return bytes;
//             }
//
//             return Array.Empty<byte>();
//         }
//
//         public T Deserialize<T>(byte[] data) where T : new()
//         {
//             return (T)Deserialize(data, typeof(T));
//         }
//
//         public object Deserialize(byte[] data, Type type)
//         {
//             if (data != null && type != null)
//             {
//                 var text = Encoding.UTF8.GetString(data);
//                 return JsonUtility.FromJson(text, type);
//             }
//
//             return null;
//         }
//
//         public string GetName()
//         {
//             return "json";
//         }
//     }
// }