// /********************************************************************
// created:    2022-08-15
// author:     lixianmin
// 基于UIEventListener挂载的事件, 如果不主动调用RemoveAllListeners()释放回调方法的话, 会导致UIWindowBase对象无法正常释放
// 所以, 现在的办法是在UIButton的OnDestroy()方法中, 主动调用onClick事件的RemoveAllListeners()方法, 其它类后续效仿.
// Copyright (C) - All Rights Reserved
// *********************************************************************/
//
// using System.Collections.Generic;
// using UnityEngine.Events;
//
// namespace Unicorn.UI
// {
//     public class UIEventListener : Disposable
//     {
//         private struct Item
//         {
//             public UnityEvent evt;
//             public UnityAction handler;
//         }
//         
//         public void AddListener(UnityEvent evt, UnityAction handler)
//         {
//             if (evt != null && handler != null)
//             {
//                 evt.AddListener(handler);
//                 items.Add(new Item{evt = evt, handler = handler});
//             }
//         }
//
//         public void RemoveAllListeners()
//         {
//             var count = items.Count;
//             if (count > 0)
//             {
//                 for (int i = 0; i < count; i++)
//                 {
//                     var item = items[i];
//                     item.evt.RemoveListener(item.handler);
//                 }
//                 
//                 items.Clear();
//             }
//         }
//
//         protected override void _DoDispose(bool isManualDisposing)
//         {
//             RemoveAllListeners();
//         }
//
//         private readonly List<Item> items = new(4);
//     }
// }