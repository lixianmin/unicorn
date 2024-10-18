
// /********************************************************************
// created:    2018-03-14
// author:     lixianmin

// Copyright (C) - All Rights Reserved
// *********************************************************************/

// using System;
// using System.Collections.Generic;
// using System.Reflection;

// namespace Unicorn
// {
//     public partial class Entity : IRemoveListener
//     {
//         public IPart AddPart(Type type)
//         {
//             if (type != null)
//             {
//                 _parts ??= new EntityTable();
//                 return _AddPart(type, true);
//             }

//             return null;
//         }

//         private IPart _AddPart(Type type, bool checkDuplicated)
//         {
//             const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
//             var constructor = type.GetConstructor(flags, null, CallingConventions.Any, Array.Empty<Type>(), null);
//             var part = constructor?.Invoke(Array.Empty<object>()) as IPart;
//             if (part != null)
//             {
// 	            if (part is IInitPart initPart)
//                 {
//                     initPart.InitPart(this);
//                 }

//                 _parts.Add(type, part, checkDuplicated);

//                 OnPartCreated?.Invoke(part);
//             }

//             return part;
//         }

//         public IPart GetPart(Type type)
// 		{
// 			var parts = _parts;
// 			if (type != null && parts != null)
// 			{
// 				return parts.GetPart(type);
// 			}

// 			return null;
// 		}

//         public void GetParts(List<IPart> results)
//         {
// 	        _parts?.GetParts(results);
//         }

//         public IPart SetDefaultPart(Type type)
// 		{
// 			if (type != null)
// 			{
// 				_parts ??= new EntityTable();
// 				var part = _parts.GetPart(type) ?? _AddPart(type, false);

// 				return part;
// 			}
// 			return null;
// 		}

//         public bool RemovePart(Type type)
// 		{
// 			var parts = _parts;
// 			if (type != null && parts != null)
// 			{
// 				return parts.Remove(type);
// 			}

// 			return false;
// 		}

//         public ListenerData AddListener(int message, Action listener)
//         {
//             if (null != listener)
//             {
//                 _observer ??= _cacheObservers.Get();
//                 _observer.AddListener(message, listener);

//                 var listenerData = new ListenerData { sender = this, message = message, listener = listener };
//                 return listenerData;
//             }

//             return default;
//         }

//         public void RemoveListener(int message, Action listener)
//         {
//             var observer = _observer;
//             if (null != observer && null != listener)
//             {
//                 observer.RemoveListener(message, listener);
//             }
//         }

//         void IRemoveListener.RemoveListener(int message, Delegate listener)
//         {
//             RemoveListener(message, listener as Action);
//         }

//         public void SendMessage(int message)
//         {
//             _observer?.SendMessage(message);
//         }

//         /// <summary>
//         /// 清理Entity状态.
//         /// Entity原本实现了Dispose()方法, 但目前评估加一个Clear()方法可能更有效. 原因是:
//         /// 1. Dispose()方法是销毁对象, 对象就无法复用了, 而Clear()方法代表对象还可以接着使用
//         /// 2. Entity本身没有让GC自动回收的动机, 如果子类有这样的需求, 则直接在子类中Dispose()相关逻辑即可
//         /// </summary>
//         public virtual void Clear()
//         {
//             _cacheObservers.Return(_observer);
//             _observer = null;

//             _parts?.Dispose();
//             _parts = null;
//         }

//         // public void Dispose()
//         // {
//         //     if (_isDisposed)
//         //     {
//         //         return;
//         //     }
//         //
//         //     _isDisposed = true;
//         //
//         //     // 将observer的回收放在_DoDispose前面，这样如果_DoDispose()中有调用RemoveListener()时，开销会变低
//         //     _cacheObservers.Recycle(_observer);
//         //     _observer = null;
//         //
//         //     _DoDispose();
//         //
//         //     _parts?.Dispose();
//         //     _parts = null;
//         // }
//         //
//         // public bool IsDisposed()
//         // {
//         //     return _isDisposed;
//         // }
//         //
//         // protected virtual void _DoDispose() { }

//         private EntityTable _parts;
//         // private bool _isDisposed;
//         private Observer _observer;

//         // global variables
//         public static event Action<IPart> OnPartCreated;
//         private static readonly ObjectPool<Observer> _cacheObservers = new(null, item => item.RemoveAllListeners());
//     }
// }