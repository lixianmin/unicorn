/********************************************************************
created:    2023-06-25
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using UnityEngine;

namespace Unicorn
{
    public static class ExtendedIHaveTransform
    {
        public static Component AddComponent(this IHaveTransform my, Type type)
        {
            var transform = my?.GetTransform();
            if (transform != null && type != null)
            {
                return transform.gameObject.AddComponent(type);
            }

            return null;
        }

        public static Component GetComponent(this IHaveTransform my, Type type)
        {
            var transform = my?.GetTransform();
            if (transform != null && type != null)
            {
                return transform.GetComponent(type);
            }

            return null;
        }

        public static GameObject GetGameObject(this IHaveTransform my)
        {
            var transform = my?.GetTransform();
            if (transform != null)
            {
                return transform.gameObject;
            }

            return null;
        }
    }
}