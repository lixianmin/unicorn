
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
        public static bool IsIntersectEx (this Vector2 area, Vector2 other)
        {
			return other.y >= area.x && other.x <= area.y;
        }
    }
}