
/********************************************************************
created:    2014-05-08
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using UnityEngine;
using System;

namespace Unicorn.Reflection
{
    public static class EditorUserBuildSettings
    {
        private static System.Reflection.PropertyInfo _activeBuildTarget;

        public static TargetPlatform activeBuildTarget
        {
            get
            {
                if (os.isEditor)
                {
                    if (null == _activeBuildTarget)
                    {
                        _activeBuildTarget = MyType.GetProperty("activeBuildTarget");
                    }

                    return (TargetPlatform)_activeBuildTarget.GetValue(null, null);
                }

                return TargetPlatform.None;
            }
        }

        private static Type _myType;

        public static Type MyType
        {
            get
            {
                if (null == _myType && os.isEditor)
                {
                    _myType = System.Type.GetType("UnityEditor.EditorUserBuildSettings,UnityEditor");
                }

                return _myType;
            }
        }
    }
}