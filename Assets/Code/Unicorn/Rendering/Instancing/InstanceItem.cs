// /********************************************************************
// created:    2023-12-27
// author:     lixianmin
//
// Copyright (C) - All Rights Reserved
// *********************************************************************/
//
// using System;
// using Unicorn.Collections;
// using UnityEngine;
//
// namespace Unicorn
// {
//     internal class InstanceItem
//     {
//         private struct MeshData
//         {
//             public Matrix4x4 matrix;
//             public Bounds bounds;
//             public int instanceId;
//         }
//
//         public InstanceItem(Mesh sharedMesh, RenderParams renderParams,
//             Slice<Matrix4x4> visibleProducer, Slice<Matrix4x4> visibleConsumer)
//         {
//             _sharedMesh = sharedMesh;
//             _renderParams = renderParams;
//             _visibleSwapper = new ThreadSwapper<Matrix4x4>(visibleProducer, visibleConsumer);
//         }
//
//         public void AddMeshRenderer(MeshRenderer renderer)
//         {
//             if (renderer != null)
//             {
//                 // 因为要使用instancing自己绘制了, 所以要关闭renderer
//                 renderer.enabled = false;
//
//                 // for Curved World, 膨胀一个, 防止穿帮
//                 // InstanceItem扩张bounds的参数改为3, 因为影子很影响视觉效果
//                 var bounds = renderer.bounds;
//                 bounds.Expand(3f);
//
//                 _dataList.Add(new MeshData
//                 {
//                     matrix = renderer.localToWorldMatrix,
//                     bounds = bounds,
//                     instanceId = renderer.GetInstanceID(),
//                 });
//             }
//         }
//
//         public bool RemoveMeshRenderer(MeshRenderer renderer)
//         {
//             if (renderer != null)
//             {
//                 var id = renderer.GetInstanceID();
//                 for (var i = 0; i < _dataList.Size; i++)
//                 {
//                     var meshData = _dataList.Items[i];
//                     if (meshData.instanceId == id)
//                     {
//                         _dataList.RemoveAt(i);
//                         renderer.enabled = true;
//                         return true;
//                     }
//                 }
//             }
//
//             return false;
//         }
//
//         // public void Render(Plane[] frustumPlanes)
//         // {
//         //     CollectVisibleMatrices(frustumPlanes);
//         //     RenderMeshInstanced();
//         // }
//
//         /// <summary>
//         /// 当前的home有近4w个对象, 需要不断的判断可见的, 这是一个计算密集型任务, 通常可能考虑三种解决方案:
//         /// 1. 使用4叉树
//         /// 2. 使用独立线程
//         /// 3. 使用compute shader
//         /// 
//         /// 本次使用是独立线程的方案, 可以完全解放主线程的计算消耗
//         /// 
//         /// 为什么不使用Job System? 是一个可以尝试的方案, 但要求所有对象都是纯struct
//         /// 为什么不不使用UniTask? 在创建action的过程中, 每一帧都会产生大量的 gc alloc
//         /// 
//         /// 在多线程处理过程中, _dataList并不会发生改变, 只需要关注frustumPlanes与_sharedVisibleMatrices的变化
//         /// 1. frustumPlanes: 长度保持为6, 内容是struct, 只有数值变化, 不会引起读写访问异常
//         /// 2. _sharedVisibleMatrices: 长度会发生变化, 需好好处理data race
//         /// </summary>
//         /// <param name="frustumPlanes"></param>
//         public void CollectVisibleMatrices(Plane[] frustumPlanes)
//         {
//             var producer = _visibleSwapper.GetProducer();
//             producer.Size = 0;
//             for (var i = 0; i < _dataList.Size; i++)
//             {
//                 var data = _dataList.Items[i];
//                 var isVisible = InstanceTools.TestPlanesAABB(frustumPlanes, data.bounds);
//                 if (isVisible)
//                 {
//                     producer.Add(data.matrix);
//                 }
//             }
//             
//             _visibleSwapper.Put(true);
//         }
//
//         public void RenderMeshInstanced()
//         {
//             var consumer = _visibleSwapper.GetConsumer();
//             consumer.Size = 0;
//             _visibleSwapper.Take(false);
//
//             // 单次推送的上限就是1023个
//             // https://docs.unity3d.com/ScriptReference/Graphics.RenderMeshInstanced.html
//             const int maxBatchSize = 1023;
//             var total = consumer.Size;
//
//             for (var i = 0; i < total; i += maxBatchSize)
//             {
//                 var size = Math.Min(maxBatchSize, total - i);
//                 // https://issuetracker.unity3d.com/issues/android-opengles-3-drawmeshinstanced-is-not-compatible-on-snapdragon-cpus-with-a-custom-render-pipeline
//                 // 这个api在Adreno GLES drivers不支持, 似乎vulkan可以, 但红米note5绘制不出来
//                 Graphics.RenderMeshInstanced(_renderParams, _sharedMesh, 0, consumer.Items, size, i);
//                 // Logo.Info($"[RenderMeshInstanced()] mesh={_sharedMesh.name}, material={_renderParams.material.name}, size={size}");
//             }
//         }
//
//         private readonly Mesh _sharedMesh;
//         private readonly RenderParams _renderParams;
//         private readonly Slice<MeshData> _dataList = new();
//
//         private readonly ThreadSwapper<Matrix4x4> _visibleSwapper;
//     }
// }