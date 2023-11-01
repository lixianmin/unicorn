
/********************************************************************
created:	2017-06-24
author:		lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using UnityEngine;

namespace Unicorn.IO
{
    public static class ExtendedIOctetsWriter
    {
        public static void Write (this IOctetsWriter writer, Vector2 v)
        {
            if (null != writer)
            {
                writer.Write(v.x, v.y);
            }
        }

        public static void Write (this IOctetsWriter writer, Vector3 v)
        {
            if (null != writer)
            {
                writer.Write(v.x, v.y, v.z);
            }
        }

        public static void Write (this IOctetsWriter writer, Vector4 v)
        {
            if (null != writer)
            {
                writer.Write(v.x, v.y, v.z, v.w);
            }
        }

        public static void Write (this IOctetsWriter writer, Color color)
        {
            if (null != writer)
            {
                if (!_IsColorRangeValid(color))
                {
                    Logo.Error("Invalid color range, color=[{0}]", color.ToString());
                }

                writer.Write(ColorTools.ToInt32(color));
            }
        }

        private static bool _IsColorRangeValid (Color color)
        {
            return _IsColorRangeValid(color.r) && _IsColorRangeValid(color.g) && _IsColorRangeValid(color.b) && _IsColorRangeValid(color.a);
        }

        private static bool _IsColorRangeValid (float f)
        {
            return f >= 0.0f && f <= 1.0f;
        }
    }
}
