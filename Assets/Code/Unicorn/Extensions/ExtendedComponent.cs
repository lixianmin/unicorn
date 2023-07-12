
/********************************************************************
created:    2014-11-01
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using UnityEngine;
using System;

namespace Unicorn
{
    //[Obfuscators.ObfuscatorIgnore]
    public static class ExtendedComponent
    {
//		public static Component GetComponentEx (this Component component, Type type)
//		{
//			if (null != component && null != type)
//			{
//				if (!os.isEditor)
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

        public static void SetActive (this Component component, bool isActive)
        {
            if (null != component)
            {
                component.gameObject.SetActive(isActive);
            }
        }
    }
}