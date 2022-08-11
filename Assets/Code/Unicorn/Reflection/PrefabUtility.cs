
/********************************************************************
created:    2017-11-27
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using UnityEngine;
using System;

namespace Unicorn.Reflection
{
    public static class PrefabUtility
    {
        private static Func<UnityEngine.Object, UnityEngine.Object> _lpfnGetPrefabParent;
        public static UnityEngine.Object GetPrefabParent (UnityEngine.Object source)
        {
            if (null == _lpfnGetPrefabParent)
            {
                var method = MyType.GetMethod("GetPrefabParent", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                TypeTools.CreateDelegate(method, out _lpfnGetPrefabParent);
            }

            return _lpfnGetPrefabParent(source);
        }

        private static Type _myType;
        public static Type MyType
        {
            get
            {
                if (null == _myType)
                {
                    _myType = System.Type.GetType("UnityEditor.PrefabUtility,UnityEditor");
                }

                return _myType;
            }
        }
    }
}
