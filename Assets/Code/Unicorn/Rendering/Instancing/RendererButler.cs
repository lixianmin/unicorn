/********************************************************************
created:    2024-02-06
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.Collections.Generic;
using Unicorn.Collections;
using UnityEngine;

namespace Unicorn.Rendering
{
    internal class RendererButler
    {
        public bool AddMeshRenderer(MeshRenderer renderer, Camera camera)
        {
            var invisible = renderer == null || !renderer.enabled || renderer.forceRenderingOff;
            if (invisible || camera == null || !_CreateInstanceKey(renderer, out var key))
            {
                return false;
            }

            if (!_items.TryGetValue(key, out var instanceItem))
            {
                var renderParams = InstanceTools.CreateRenderParams(key.material, renderer, camera);
                instanceItem = new InstanceItem(key.mesh, renderParams);
                _items.Add(key, instanceItem);
            }

            instanceItem.AddMeshRenderer(renderer);
            return true;
        }

        public bool RemoveMeshRender(MeshRenderer renderer)
        {
            if (renderer == null)
            {
                return false;
            }

            if (_CreateInstanceKey(renderer, out var key) && _items.TryGetValue(key, out var instanceItem))
            {
                return instanceItem.RemoveMeshRenderer(renderer);
            }

            return false;
        }

        private bool _CreateInstanceKey(MeshRenderer renderer, out InstanceKey key)
        {
            key = default;
            var meshFilter = renderer.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                return false;
            }

            var sharedMesh = meshFilter.sharedMesh;
            var sharedMaterial = renderer.sharedMaterial;
            if (sharedMesh == null || sharedMaterial == null || !sharedMaterial.enableInstancing)
            {
                return false;
            }

            key = new InstanceKey
            {
                mesh = sharedMesh,
                material = sharedMaterial,
            };

            return true;
        }

        public void FetchInstanceItems(Slice<InstanceItem> items)
        {
            foreach (var pair in _items)
            {
                items.Add(pair.Value);
            }
        }

        public void Clear()
        {
            _items.Clear();
        }

        private readonly Dictionary<InstanceKey, InstanceItem> _items = new();
    }
}