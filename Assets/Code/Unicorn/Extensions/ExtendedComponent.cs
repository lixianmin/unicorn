/********************************************************************
created:    2014-11-01
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using System;

namespace Unicorn
{
    public static class ExtendedComponent
    {
        public static Component AddComponent(this Component component, Type type)
        {
            if (component != null && type != null)
            {
                var go = component.gameObject;
                if (go != null)
                {
                    return go.AddComponent(type);
                }
            }

            return null;
        }

        public static T AddComponent<T>(this Component component) where T : Component
        {
            if (component != null)
            {
                var go = component.gameObject;
                if (go != null)
                {
                    return go.AddComponent(typeof(T)) as T;
                }
            }

            return null;
        }

        public static Component SetDefaultComponent(this Component component, Type type)
        {
            if (component != null)
            {
                return component.gameObject.SetDefaultComponent(type);
            }

            return null;
        }

        public static T SetDefaultComponent<T>(this Component component) where T : Component
        {
            if (component != null)
            {
                return component.gameObject.SetDefaultComponent<T>();
            }

            return null;
        }

//		public static Component GetComponentEx (this Component component, Type type)
//		{
//			if (null != component && null != type)
//			{
//				if (!Application.isEditor)
//				{
//					var result = component.GetComponent(type);
//					return result;
//				}
//				else
//				{
//                    //var result = component.gameObject.GetComponentEx(type);
//                    var result = component.GetComponent(type);
//					return result;
//				}
//			}
//
//			return null;
//		}

        public static void SetActive(this Component component, bool isActive)
        {
            if (null != component)
            {
                component.gameObject.SetActive(isActive);
            }
        }
    }
}