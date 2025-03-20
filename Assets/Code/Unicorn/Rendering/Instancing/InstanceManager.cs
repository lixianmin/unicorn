// /********************************************************************
// created:    2023-12-27
// author:     lixianmin
//
// Copyright (C) - All Rights Reserved
// *********************************************************************/
//
// using System;
// using System.Threading;
// using Unicorn.Collections;
// using UnityEngine;
// using Object = UnityEngine.Object;
//
// namespace Unicorn
// {
//     public class InstanceManager
//     {
//         private InstanceManager()
//         {
//         }
//
//         public void RefreshRenderers(Predicate<MeshRenderer> canGpuInstancing = null)
//         {
//             _mainCamera = Camera.main;
//             _butler.Clear();
//             _SetDirty();
//
//             if (_mainCamera == null || !IsEnabled())
//             {
//                 Logo.Warn($"_mainCamera={_mainCamera}, IsEnabled={IsEnabled()}");
//                 return;
//             }
//
//             // 10ms 找了近2w个物体, 不算最慢的一环
//             var meshRenderers = Object.FindObjectsOfType<MeshRenderer>();
//             foreach (var meshRenderer in meshRenderers)
//             {
//                 _butler.AddMeshRenderer(meshRenderer, _mainCamera, canGpuInstancing);
//             }
//         }
//
//         public void AddRenderer(MeshRenderer renderer, Predicate<MeshRenderer> canGpuInstancing = null)
//         {
//             if (IsEnabled() && _butler.AddMeshRenderer(renderer, _mainCamera, canGpuInstancing))
//             {
//                 _SetDirty();
//             }
//         }
//
//         public void RemoveRenderer(MeshRenderer renderer)
//         {
//             if (IsEnabled() && _butler.RemoveMeshRender(renderer))
//             {
//                 _SetDirty();
//             }
//         }
//
//         internal void LateUpdate()
//         {
//             if (IsEnabled() && _mainCamera != null)
//             {
//                 if (_isDirty)
//                 {
//                     _isDirty = false;
//                     _RefreshItems();
//                 }
//
//                 GeometryUtility.CalculateFrustumPlanes(_mainCamera, _frustumPlanes);
//                 foreach (var item in _instanceSwapper.GetProducer())
//                 {
//                     item.RenderMeshInstanced();
//                 }
//             }
//         }
//
//         private void _Run()
//         {
//             // 主副线程交换数据这个设计模式, 需要三个一模一样的List/Slice, 一个在主线程中, 一个在副线程中, 一个shared用于交换
//             var instanceConsumer = _instanceSwapper.GetConsumer();
//             var lastTick = Environment.TickCount;
//
//             const int stepTick = 1000 / 30;
//
//             while (IsEnabled())
//             {
//                 var nextTick = Environment.TickCount;
//                 var sleepTime = stepTick - nextTick + lastTick;
//                 lastTick = nextTick;
//
//                 if (sleepTime > 0)
//                 {
//                     Thread.Sleep(sleepTime);
//                     lastTick = Environment.TickCount;
//                     // Logo.Info($"stepTick={stepTick}, deltaTime={deltaTime}, sleepTime={sleepTime}");
//                 }
//
//                 try
//                 {
//                     instanceConsumer.Clear();
//                     _instanceSwapper.Take(false);
//
//                     foreach (var item in instanceConsumer)
//                     {
//                         item.CollectVisibleMatrices(_frustumPlanes);
//                     }
//                 }
//                 catch (Exception)
//                 {
//                     // ignored
//                 }
//             }
//             // ReSharper disable once FunctionNeverReturns
//         }
//
//         private void _SetDirty()
//         {
//             _isDirty = true;
//         }
//
//         private void _RefreshItems()
//         {
//             var producer = _instanceSwapper.GetProducer();
//             producer.Clear();
//             _butler.FetchInstanceItems(producer);
//
//             _instanceSwapper.Put(true);
//             Logo.Info($"[_RefreshItems()] _instanceItems.Count={producer.Size}");
//         }
//
//         public bool IsEnabled()
//         {
//             return Interlocked.Read(ref _isEnabled) == 1;
//         }
//
//         public void Enable(bool enable)
//         {
//             if (enable != IsEnabled())
//             {
//                 var v = enable ? 1 : 0;
//                 Interlocked.Exchange(ref _isEnabled, v);
//
//                 // 如果频繁调用Enable(true), 有同时启动两个线程的风险, 目前应用场景只在初始化的时候启动一次, 先这样吧
//                 if (enable)
//                 {
//                     var thread = new Thread(_Run);
//                     thread.Start();
//                 }
//
//                 Logo.Info($"[InstanceManager.Enable()] IsEnabled={IsEnabled()}");
//             }
//         }
//
//         private readonly ThreadSwapper<InstanceItem> _instanceSwapper = new();
//         private readonly RendererButler _butler = new();
//
//         private const int FrustumPlaneNum = 6;
//         private readonly Plane[] _frustumPlanes = new Plane[FrustumPlaneNum];
//         private Camera _mainCamera;
//         private bool _isDirty;
//         private long _isEnabled;
//
//         public static readonly InstanceManager It = new();
//     }
// }