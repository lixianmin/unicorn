/********************************************************************
created:    2024-02-06
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.Collections.Generic;
using Unicorn.Collections;
using UnityEngine;

namespace Unicorn
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
            var sharedMaterials = renderer.sharedMaterials;
            if (sharedMesh == null || sharedMaterials is not { Length: 1 } || !sharedMaterials[0].enableInstancing)
            {
                return false;
            }

            key = new InstanceKey
            {
                mesh = sharedMesh,
                material = sharedMaterials[0],
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
        
        // 做mesh合并, 把所有相同material的mesh合到一起, 有可能会进一步降低draw call, 只是这个要求mesh打开isReadable开关,
        // 默认情况下是不开的, 打开后似乎是双倍内存
        // 
        // private class Info
        // {
        //     public Slice<MeshFilter> meshFilters;
        //     public Slice<InstanceItem3.MeshData> dataList;
        //     public RenderParams renderParams;
        // }
        //
        // public void FetchInstanceItems(Slice<InstanceItem3> items)
        // {
        //     var infoDict = new Dictionary<Material, Info>();
        //     
        //     foreach (var pair in _items)
        //     {
        //         var item = pair.Value;
        //         var material = item.GetMaterial();
        //         if (!infoDict.TryGetValue(material, out var info))
        //         {
        //             info = new Info()
        //             {
        //                 meshFilters = new Slice<MeshFilter>(),
        //                 dataList = new Slice<InstanceItem3.MeshData>(),
        //                 renderParams = item.GetRenderParams(),
        //             };
        //             
        //             infoDict.Add(material, info);
        //         }
        //
        //         info.dataList.AddRange(item.GetDataList());
        //         
        //         foreach (var meshFilter in item.GetMeshFilters())
        //         {
        //             info.meshFilters.Add(meshFilter);
        //         }
        //     }
        //     
        //     // var combines = new Dictionary<Material, Mesh>();
        //     foreach (var pair in infoDict)
        //     {
        //         var info = pair.Value;
        //         var combinedMesh = _CombineMeshes(info.meshFilters.Items, info.meshFilters.Count);
        //         
        //         var item = new InstanceItem3(combinedMesh, info.renderParams, info.dataList);
        //         items.Add(item);
        //     }
        // }

        public void Clear()
        {
            _items.Clear();
        }

        private readonly Dictionary<InstanceKey, InstanceItem> _items = new();
    }
}