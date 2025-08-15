/********************************************************************
created:    2014-11-01
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using UnityEngine;

namespace Unicorn
{
    public static class ExtendedGameObject
    {
        public static Component SetDefaultComponent(this GameObject go, Type type)
        {
            if (null != go && null != type)
            {
                if (!go.TryGetComponent(type, out var widget))
                {
                    widget = go.AddComponent(type);
                }

                return widget;
            }

            return null;
        }

        public static void SetLayerRecursively(this GameObject go, int layer)
        {
            if (null != go)
            {
                go.layer = layer;
                foreach (Transform child in go.transform)
                {
                    SetLayerRecursively(child.gameObject, layer);
                }
            }
        }
    }
}