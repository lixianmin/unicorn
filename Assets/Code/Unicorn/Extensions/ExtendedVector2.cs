﻿/********************************************************************
created:	2023-07-09
author:		lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;

namespace Unicorn
{
    public static class ExtendedVector2
    {
        // public static bool IsIntersect(this Vector2 area, Vector2 other)
        // {
        //     return other.y >= area.x && other.x <= area.y;
        // }

        public static bool IsZero(this Vector2 my, float eps = Eps)
        {
            return my.x.IsZero(eps) && my.y.IsZero(eps);
        }

        public static bool IsEqual(this Vector2 my, Vector2 other, float eps = Eps)
        {
            var delta = my - other;
            return delta.IsZero(eps);
        }

        private const float Eps = 0.000001f;
    }
}