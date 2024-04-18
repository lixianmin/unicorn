/********************************************************************
created:    2024-01-30
author:     lixianmin

https://forum.unity.com/threads/managed-version-of-geometryutility-testplanesaabb.473575/

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unicorn
{
    public class InstanceTools
    {
        /// <summary>
        /// 为什么不直接使用: GeometryUtility.TestPlanesAABB(frustumPlanes, data.bounds); // 无法在独立线程中调用
        /// </summary>
        /// <param name="planes"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static bool TestPlanesAABB(Plane[] planes, Bounds bounds)
        {
            foreach (var plane in planes)
            {
                var normal = plane.normal;
                var normalSign = new Vector3(_Sign(normal.x), _Sign(normal.y), _Sign(normal.z));
                var extents = bounds.extents;
                var testPoint = bounds.center + new Vector3(
                    extents.x * normalSign.x,
                    extents.y * normalSign.y,
                    extents.z * normalSign.z);

                var dotted = _Dot(testPoint, normal);
                if (dotted + plane.distance < 0)
                {
                    return false;
                }
            }

            return true;
        }

        // public static bool TestPlanesAABB(NativeArray<Plane> planes, Bounds bounds)
        // {
        //     foreach (var plane in planes)
        //     {
        //         var normalSign = math.sign(plane.normal);
        //         var testPoint = (float3)bounds.center + bounds.extents * normalSign;
        //
        //         var dot = math.dot(testPoint, plane.normal);
        //         if (dot + plane.distance < 0)
        //         {
        //             return false;
        //         }
        //     }
        //
        //     return true;
        // }

        public static RenderParams CreateRenderParams(Material material, Renderer renderer, Camera camera = null)
        {
            var renderParams = new RenderParams(material);
            if (renderer != null)
            {
                renderParams.shadowCastingMode = renderer.shadowCastingMode;
            }

            // 如果不设置, 就会在所有camera中渲染一遍, 这包括UICamera, 所以在release的时候必须要设置
            // 但如果一直设置, 在Editor中的Scene窗口就看不到了, 所以在Editor中倾向于不设置
            if (!Application.isEditor)
            {
                renderParams.camera = camera;
            }

            return renderParams;
        }
        
                
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float _Sign(float x)
        {
            return (x > 0.0f ? 1.0f : 0.0f) - (x < 0.0f ? 1.0f : 0.0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float _Dot(Vector3 x, Vector3 y)
        {
            return x.x * y.x + x.y * y.y + x.z * y.z;
        }

        // public enum TestPlanesResults
        // {
        //     /// <summary>
        //     /// The AABB is completely in the frustum.
        //     /// </summary>
        //     Inside = 0,
        //
        //     /// <summary>
        //     /// The AABB is partially in the frustum.
        //     /// </summary>
        //     Intersect,
        //
        //     /// <summary>
        //     /// The AABB is completely outside the frustum.
        //     /// </summary>
        //     Outside
        // }
        //
        // /// <summary>
        // /// This is crappy performant, but easiest version of TestPlanesAABBFast to use.
        // /// </summary>
        // /// <param name="planes"></param>
        // /// <param name="bounds"></param>
        // /// <returns></returns>
        // public static TestPlanesResults TestPlanesAABBInternalFast(Plane[] planes, ref Bounds bounds)
        // {
        //     var min = bounds.min;
        //     var max = bounds.max;
        //
        //     return TestPlanesAABBInternalFast(planes, ref min, ref max);
        // }
        //
        // /// <summary>
        // /// This is a faster AABB cull than brute force that also gives additional info on intersections.
        // /// Calling Bounds.Min/Max is actually quite expensive so as an optimization you can precalculate these.
        // /// http://www.lighthouse3d.com/tutorials/view-frustum-culling/geometric-approach-testing-boxes-ii/
        // /// </summary>
        // /// <param name="planes"></param>
        // /// <param name="boundsMin"></param>
        // /// <param name="boundsMax"></param>
        // /// <returns></returns>
        // public static TestPlanesResults TestPlanesAABBInternalFast(Plane[] planes, ref Vector3 boundsMin,
        //     ref Vector3 boundsMax, bool testIntersection = false)
        // {
        //     var testResult = TestPlanesResults.Inside;
        //
        //     for (var planeIndex = 0; planeIndex < planes.Length; planeIndex++)
        //     {
        //         var normal = planes[planeIndex].normal;
        //         var planeDistance = planes[planeIndex].distance;
        //
        //         // X axis
        //         Vector3 vmin;
        //         Vector3 vmax;
        //         if (normal.x < 0)
        //         {
        //             vmin.x = boundsMin.x;
        //             vmax.x = boundsMax.x;
        //         }
        //         else
        //         {
        //             vmin.x = boundsMax.x;
        //             vmax.x = boundsMin.x;
        //         }
        //
        //         // Y axis
        //         if (normal.y < 0)
        //         {
        //             vmin.y = boundsMin.y;
        //             vmax.y = boundsMax.y;
        //         }
        //         else
        //         {
        //             vmin.y = boundsMax.y;
        //             vmax.y = boundsMin.y;
        //         }
        //
        //         // Z axis
        //         if (normal.z < 0)
        //         {
        //             vmin.z = boundsMin.z;
        //             vmax.z = boundsMax.z;
        //         }
        //         else
        //         {
        //             vmin.z = boundsMax.z;
        //             vmax.z = boundsMin.z;
        //         }
        //
        //         var dot1 = normal.x * vmin.x + normal.y * vmin.y + normal.z * vmin.z;
        //         if (dot1 + planeDistance < 0)
        //         {
        //             return TestPlanesResults.Outside;
        //         }
        //
        //         if (testIntersection)
        //         {
        //             var dot2 = normal.x * vmax.x + normal.y * vmax.y + normal.z * vmax.z;
        //             if (dot2 + planeDistance <= 0)
        //             {
        //                 testResult = TestPlanesResults.Intersect;
        //             }
        //         }
        //     }
        //
        //     return testResult;
        // }
    }
}