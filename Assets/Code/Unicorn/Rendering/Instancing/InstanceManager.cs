/********************************************************************
created:    2023-12-27
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Threading;
using Unicorn.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unicorn.Rendering
{
    public class InstanceManager
    {
        private InstanceManager()
        {
            var thread = new Thread(_Run);
            thread.Start();
        }

        public void RefreshMeshRenderers()
        {
            _mainCamera = Camera.main;
            _butler.Clear();
            _SetDirty();

            if (_mainCamera == null)
            {
                return;
            }

            // 10ms 找了近2w个物体, 不算最慢的一环
            var meshRenderers = Object.FindObjectsOfType<MeshRenderer>();
            foreach (var meshRenderer in meshRenderers)
            {
                _butler.AddMeshRenderer(meshRenderer, _mainCamera);
            }
        }

        public void AddMeshRenderer(MeshRenderer renderer)
        {
            if (_butler.AddMeshRenderer(renderer, _mainCamera))
            {
                _SetDirty();
            }
        }

        public void RemoveMeshRenderer(MeshRenderer renderer)
        {
            if (_butler.RemoveMeshRender(renderer))
            {
                _SetDirty();
            }
        }

        public void ExpensiveUpdate()
        {
            if (_mainCamera != null)
            {
                if (_isDirty)
                {
                    _isDirty = false;
                    _RefreshItems();
                }
                
                GeometryUtility.CalculateFrustumPlanes(_mainCamera, _frustumPlanes);
                foreach (var item in _instanceItems)
                {
                    item.RenderMeshInstanced(_tempVisibleMatrices);
                }
            }
        }

        private void _Run()
        {
            var receivedItems = new Slice<InstanceItem>();
            var tempVisibleMatrices = new Slice<Matrix4x4>();
            var lastTick = Environment.TickCount;

            const int stepTick = 1000 / 30;

            while (true)
            {
                var nextTick = Environment.TickCount;
                var sleepTime = stepTick - nextTick + lastTick;
                lastTick = nextTick;

                if (sleepTime > 0)
                {
                    Thread.Sleep(sleepTime);
                    lastTick = Environment.TickCount;
                    // Logo.Info($"stepTick={stepTick}, deltaTime={deltaTime}, sleepTime={sleepTime}");
                }

                try
                {
                    lock (_locker)
                    {
                        receivedItems.Clear();
                        receivedItems.AddRange(_sharedItems);
                    }

                    foreach (var item in receivedItems)
                    {
                        item.CollectVisibleMatrices(_frustumPlanes, tempVisibleMatrices);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void _SetDirty()
        {
            _isDirty = true;
        }
        
        private void _RefreshItems()
        {
            _instanceItems.Clear();
            _butler.FetchInstanceItems(_instanceItems);

            lock (_locker)
            {
                _sharedItems.Clear();
                _sharedItems.AddRange(_instanceItems);
            }
        }

        private readonly object _locker = new();
        private readonly Slice<InstanceItem> _sharedItems = new();

        private readonly Slice<InstanceItem> _instanceItems = new();
        private readonly Slice<Matrix4x4> _tempVisibleMatrices = new();
        private readonly RendererButler _butler = new();

        private const int FrustumPlaneNum = 6;
        private readonly Plane[] _frustumPlanes = new Plane[FrustumPlaneNum];
        private Camera _mainCamera;
        private bool _isDirty;
        
        public bool IsEnabled;
        public static readonly InstanceManager It = new();
    }
}