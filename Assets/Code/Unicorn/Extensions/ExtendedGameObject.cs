/********************************************************************
created:    2013-12-18
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;

namespace Unicorn
{
    public static class ExtendedGameObject
    {
        public static void GetComponentsInChildren(this GameObject go, string type, bool includeInactive, List<Component> results)
        {
            if (null != go && !string.IsNullOrEmpty(type) && null != results)
            {
                _GetComponentsInChildren(go, type, includeInactive, results);
            }
        }

        private static void _GetComponentsInChildren(GameObject go, string type, bool includeInactive, List<Component> results)
        {
            if (includeInactive || go.activeSelf)
            {
                var component = go.GetComponent(type);
                if (null != component)
                {
                    results.Add(component);
                }

                var parent = go.transform;
                var count = parent.childCount;
                for (int i = 0; i < count; ++i)
                {
                    var child = parent.GetChild(i);
                    _GetComponentsInChildren(child.gameObject, type, includeInactive, results);
                }
            }
        }

//        public static T GetComponentInParentEx<T> (this GameObject go) where T : Component
//        {
//            if (null == go)
//            {
//                return null;
//            }
//            
//            var component = go.GetComponentEx<T>();
//            if (null != component)
//            {
//                return component;
//            }
//            
//            var trans   = go.transform.parent;
//            while (null != trans)
//            {
//                component = trans.GetComponentEx<T>();
//                
//                if(null != component)
//                {
//                    return component;
//                }
//                
//                trans = trans.parent;
//            }
//            
//            return null;
//        }

        // public static void SetActive (this GameObject go, bool active)
        // {
        //     if(null != go && go.activeSelf != active)
        //     {
        //         go.SetActive(active);
        //     }
        // }

        public static void SetLayerRecursively(this GameObject go, int layer)
        {
            if (null != go)
            {
                go.layer = layer;
                foreach (Transform child in go.transform)
                {
                    child.gameObject.SetLayerRecursively(layer);
                }
            }
        }

//        public static bool GetActiveInHierarchyEx (this GameObject go)
//        {
//            if (null != go)
//            {
//                return go.activeInHierarchy;
//            }
//            else
//            {
//                Logo.Error("[GameObject:GetActiveInHierarchyEx()] go is null");
//                return false;
//            }
//        }
//
//        public static bool GetActiveSelfEx (this GameObject go)
//        {
//            if (null != go)
//            {
//                return go.activeSelf;
//            }
//            else
//            {
//                Logo.Error("[GameObject:GetActiveSelfEx()] go is null");
//                return false;
//            }
//        }

        public static Component SetDefaultComponent(this GameObject go, Type type)
        {
            if (null != go && null != type)
            {
                var component = go.GetComponent(type);

                if (null == component)
                {
                    component = go.AddComponent(type);
                }

                return component;
            }

            return null;
        }

        public static T SetDefaultComponent<T>(this GameObject go) where T : Component
        {
            if (null != go)
            {
                var component = go.GetComponent<T>();

                if (null == component)
                {
                    component = go.AddComponent<T>();
                }

                return component;
            }

            return null;
        }
    }
}