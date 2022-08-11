
/********************************************************************
created:    2017-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using UnityEngine;

namespace Unicorn
{
    public static class ExtendedLightmapData
    {
        public static void SetLightmapColorEx (this LightmapData data, Texture2D lightmapColor)
        {
            if (null == data)
            {
                return;
            }

            #if UNITY_2017_1_OR_NEWER
            data.lightmapColor = lightmapColor;
            #else
            data.lightmapFar = lightmapColor;
            #endif
        }

        public static Texture2D GetLightmapColorEx (this LightmapData data)
        {
            if (null == data)
            {
                return null;
            }

            #if UNITY_2017_1_OR_NEWER
            return data.lightmapColor;
            #else
            return data.lightmapFar;
            #endif
        }

        public static void SetLightmapDirEx (this LightmapData data, Texture2D lightmapDir)
        {
            if (null == data)
            {
                return;
            }

            #if UNITY_2017_1_OR_NEWER
            data.lightmapDir = lightmapDir;
            #else
            data.lightmapNear = lightmapDir;
            #endif
        }

        public static Texture2D GetLightmapDirEx (this LightmapData data)
        {
            if (null == data)
            {
                return null;
            }

            #if UNITY_2017_1_OR_NEWER
            return data.lightmapDir;
            #else
            return data.lightmapNear;
            #endif
        }
    }
}
