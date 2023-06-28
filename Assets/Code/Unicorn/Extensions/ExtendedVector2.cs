/********************************************************************
created:	2015-02-07
author:		lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;

namespace Unicorn
{
    //[Obfuscators.ObfuscatorIgnore]
    public static class ExtendedVector2
    {
        public static bool IsIntersect(this Vector2 area, Vector2 other)
        {
            return other.y >= area.x && other.x <= area.y;
        }

        public static bool IsZero(this Vector2 my, float eps = 0.000001f)
        {
            return my.x.IsZero(eps) && my.y.IsZero(eps);
        }
    }
}