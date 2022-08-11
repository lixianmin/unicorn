
/*********************************************************************
created:    2015-01-24
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using UnityEngine;

namespace Unicorn
{
    public static class ColorTools
    {
        public static int ToInt32(Color color)
        {
            return ((int)(color.r * 255f))
                    | ((int)(color.g * 255f) << 8)
                    | ((int)(color.b * 255f) << 16)
                    | ((int)(color.a * 255f) << 24);
        }

        public static Color FromInt32(int v)
        {
            var color = new Color
            {
                r = ((byte)v) / 255f,
                g = ((byte)(v >> 8)) / 255f,
                b = ((byte)(v >> 16)) / 255f,
                a = ((byte)(v >> 24)) / 255f
            };

            return color;
        }

        private static bool _IsEqual(float a, float b)
        {
            // 1/255.0f = 0.003922;
            var eps = 0.003922f;
            var delta = a - b;
            delta = delta >= 0.0f ? delta : -delta;
            return delta < eps;
        }

        public static bool IsEqual(Color left, Color right)
        {
            return _IsEqual(left.r, right.r)
                    && _IsEqual(left.g, right.g)
                    && _IsEqual(left.b, right.b)
                    && _IsEqual(left.a, right.a);
        }
    }
}