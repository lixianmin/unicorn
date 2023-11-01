
/********************************************************************
created:	2017-06-24
author:		lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using UnityEngine;

namespace Unicorn.IO
{
    public static class ExtendedIOctetsReader
    {
        public static Vector2 ReadVector2 (this IOctetsReader reader)
        {
            if (null != reader)
            {
                Vector2 v;
                reader.ReadVector(out v.x, out v.y);
                return v;
            }

            return Vector2.zero;
        }

        public static Vector3 ReadVector3 (this IOctetsReader reader)
        {
            if (null != reader)
            {
                Vector3 v;
                reader.ReadVector(out v.x, out v.y, out v.z);
                return v;
            }

            return Vector3.zero;
        }

        public static Vector4 ReadVector4 (this IOctetsReader reader)
        {
            if (null != reader)
            {
                Vector4 v;
                reader.ReadVector(out v.x, out v.y, out v.z, out v.w);
                return v;
            }

            return Vector4.zero;
        }

        public static Color ReadColor (this IOctetsReader reader)
        {
            if (null != reader)
            {
                return ColorTools.FromInt32(reader.ReadInt32());
            }

            return Color.black;
        }
    }
}
