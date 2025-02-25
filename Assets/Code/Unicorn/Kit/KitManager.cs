//
// /********************************************************************
// created:    2022-09-05
// author:     lixianmin
//
// 因为 Behavior Designer, 我们可能没有对KitManager的需求了, 因此这部分代码封禁
//
// Copyright (C) - All Rights Reserved
// *********************************************************************/
//
// using System;
// using System.Collections.Generic;
//
// namespace Unicorn.Kit
// {
//     internal class KitManager
//     {
//         private class KitComparer : IComparer<KitBase>
//         {
//             public int Compare(KitBase a, KitBase b)
//             {
//                 return a!.sort - b!.sort;
//             }
//         }
//         
//         static KitManager()
//         {
//         }
//
//         private KitManager()
//         {
//         }
//
//         public void SlowUpdate(float deltaTime)
//         {
//             if (_dirty)
//             {
//                 Array.Sort(_kits, 0, _size, _comparer);
//                 _dirty = false;
//             }
//
//             var hasDisposed = _SlowUpdateKits(deltaTime);
//             if (hasDisposed)
//             {
//                 _RemoveDisposedKits();
//             }
//         }
//         
//         private bool _SlowUpdateKits(float deltaTime)
//         {
//             var count = _size; // 在update的过程中, 即使_size大小改变了也无影响
//             var hasDisposed = false;
//             for (var i = 0; i < count; ++i)
//             {
//                 var kit = _kits[i];
//                 var disposed = (IIsDisposed)kit;
//                 if (!disposed.IsDisposed())
//                 {
//                     kit.InnerSlowUpdate(deltaTime);
//                 }
//                 else 
//                 {
//                     hasDisposed = true;
//                 }
//             }
//
//             return hasDisposed;
//         }
//
//         public void ExpensiveUpdate(float deltaTime)
//         {
//             if (_dirty)
//             {
//                 Array.Sort(_kits, 0, _size, _comparer);
//                 _dirty = false;
//             }
//
//             var hasDisposed = _ExpensiveUpdateKits(deltaTime);
//             if (hasDisposed)
//             {
//                 _RemoveDisposedKits();
//             }
//         }
//
//         private bool _ExpensiveUpdateKits(float deltaTime)
//         {
//             var count = _size; // 在update的过程中, 即使_size大小改变了也无影响
//             var hasDisposed = false;
//             for (var i = 0; i < count; ++i)
//             {
//                 var kit = _kits[i];
//                 var disposed = (IIsDisposed)kit;
//                 if (!disposed.IsDisposed())
//                 {
//                     if (kit.isActiveAndEnabled)
//                     {
//                         var updater = kit as IExpensiveUpdater;
//                         updater?.ExpensiveUpdate(deltaTime);
//                     }
//                 }
//                 else 
//                 {
//                     hasDisposed = true;
//                 }
//             }
//
//             return hasDisposed;
//         }
//         
//         private void _RemoveDisposedKits()
//         {
//             int i;
//             for (i = 0; i < _size; i++)
//             {
//                 var kit = _kits[i];
//                 var disposed = (IIsDisposed) kit ;
//                 if (disposed.IsDisposed())
//                 {
//                     break;
//                 }
//             }
//
//             if (i == _size)
//             {
//                 return;
//             }
//
//             int j;
//             for (j = i + 1; j < _size; j++)
//             {
//                 var kit = _kits[j];
//                 var disposed = (IIsDisposed) kit ;
//                 if (!disposed.IsDisposed())
//                 {
//                     _kits[i] = _kits[j];
//                     ++i;
//                 }
//             }
//
//             var removedCount = j - i;
//             if (removedCount > 0)
//             {
//                 Array.Clear(_kits, i, removedCount);
//             }
//
//             _size = i;
//         }
//         
//         public void Add(KitBase kit)
//         {
//             if (kit is not null)
//             {
//                 if (_size == _capacity)
//                 {
//                     _capacity <<= 1;
//                     
//                     var kits = new KitBase[_capacity];
//                     Array.Copy(_kits, 0, kits, 0, _size);
//                     
//                     _kits = kits;
//                 }
//                 
//                 _kits[_size] = kit;
//                 ++_size;
//                 SetDirty();
//             }
//         }
//
//         private void SetDirty()
//         {
//             _dirty = true;
//         }
//         
//         private int _size;
//         private int _capacity = 4;
//         private bool _dirty;
//
//         private KitBase[] _kits = new KitBase[4];
//         private readonly KitComparer _comparer = new();
//
//         public static readonly KitManager It = new ();
//     }
// }