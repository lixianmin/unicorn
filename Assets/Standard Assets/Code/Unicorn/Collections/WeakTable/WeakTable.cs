//
// /*********************************************************************
// created:    2022-08-11
// author:     lixianmin
//
// 这个类原用于MetaTools中记录FieldInfo[], 但当Template结构变化的时候, 往往会返回
// 错误的FieldInfo[], 所以不再使用了
//
// Copyright (C) - All Rights Reserved
// *********************************************************************/
// #pragma warning disable 0414
// using System;
// using System.Collections;
//
// namespace Unicorn.Collections
// {
//     public class WeakTable
//     {
//         public WeakTable () : this(0)
//         {
//             
//         }
//
//         public WeakTable (int capacity)
//         {
//             _OnGC.AddListener(_OnGCHandler, out _lpfnOnGCHandler);
//             _OnGC.RemoveListener(_OnGCHandler);
//         }
//
// 		internal void Clear ()
// 		{
// 			_items.Clear();
// 		}
//
//         private void _OnGCHandler (int arg)
//         {
//             _items.Clear();
//         }
//
//         public static void CollectGarbage ()
//         {
//             _OnGC.Call(0);
//         }
//
//         public object this [object key]
//         {
//             get
//             {
//                 var target = _items[key];
//                 return target;
//             }
//
//             set => _items[key] = value;
//         }
//
//         public int Count => _items.Count;
//
//         private readonly Hashtable _items = new();
//         private readonly Action<int> _lpfnOnGCHandler;
//
//         private static readonly SafeEvent<int> _OnGC = new();
//     }
// }