// /********************************************************************
// created:    2022-08-26
// author:     lixianmin
//
// Copyright (C) - All Rights Reserved
// *********************************************************************/
//
// using System;
// using UnityEngine;
// using UObject = UnityEngine.Object;
//
// namespace Unicorn.Kit
// {
//     partial class KitBase
//     {
//         internal void InnerInit(Transform transform, UObject[] assets)
//         {
//             _transform = transform;
//             _assets = assets;
//
//             KitManager.It.Add(this);
//
//             // 调用Awake()方法
//             try
//             {
//                 Awake();
//             }
//             catch (Exception ex)
//             {
//                 Logo.Error($"ex= {ex}");
//             }
//         }
//
//         internal void InnerEnable()
//         {
//             isActiveAndEnabled = true;
//             try
//             {
//                 OnEnable();
//             }
//             catch (Exception ex)
//             {
//                 Logo.Error($"ex= {ex}");
//             }
//         }
//
//         internal void InnerDisable()
//         {
//             isActiveAndEnabled = false;
//             try
//             {
//                 OnDisable();
//             }
//             catch (Exception ex)
//             {
//                 Logo.Error($"ex= {ex}");
//             }
//         }
//
//         internal void InnerTriggerEnter(Collider other)
//         {
//             try
//             {
//                 OnTriggerEnter(other);
//             }
//             catch (Exception ex)
//             {
//                 Logo.Error($"ex= {ex}");
//             }
//         }
//
//         internal void InnerTriggerExit(Collider other)
//         {
//             try
//             {
//                 OnTriggerExit(other);
//             }
//             catch (Exception ex)
//             {
//                 Logo.Error($"ex= {ex}");
//             }
//         }
//
//         internal void InnerDispose()
//         {
//             CallbackTools.Handle(OnDestroy, "[OnDestroy()]");
//             // _listener.RemoveAllListeners();
//             _isDisposed = true;
//         }
//
//         internal void InnerSlowUpdate(float deltaTime)
//         {
//             if (isActiveAndEnabled)
//             {
//                 SlowUpdate(deltaTime);
//             }
//         }
//     }
// }