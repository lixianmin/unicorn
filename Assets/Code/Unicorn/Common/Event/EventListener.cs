// /********************************************************************
// created:    2023-09-01
// author:     lixianmin
//  这个名字有点跟C#自带的有点儿重复了啊
// Copyright (C) - All Rights Reserved
// *********************************************************************/
//
// using System;
//
// namespace Unicorn
// {
//     public class EventListener : IEventListener
//     {
//         public void AddListener(Action handler)
//         {
//             if (handler != null)
//             {
//                 _event += handler;
//             }
//         }
//
//         public void RemoveListener(Action handler)
//         {
//             if (handler != null)
//             {
//                 _event -= handler;
//             }
//         }
//
//         public void RemoveAllListeners()
//         {
//             _event = null;
//         }
//
//         public void Invoke()
//         {
//             _event?.Invoke();
//         }
//
//         private event Action _event;
//     }
//     
//     public class EventListener<T> : IEventListener<T>
//     {
//         public void AddListener(Action<T> handler)
//         {
//             if (handler != null)
//             {
//                 _event += handler;
//             }
//         }
//
//         public void RemoveListener(Action<T> handler)
//         {
//             if (handler != null)
//             {
//                 _event -= handler;
//             }
//         }
//
//         public void RemoveAllListeners()
//         {
//             _event = null;
//         }
//
//         public void Invoke(T arg0)
//         {
//             _event?.Invoke(arg0);
//         }
//
//         private event Action<T> _event;
//     }
// }