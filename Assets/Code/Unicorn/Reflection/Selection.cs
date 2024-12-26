/********************************************************************
created:    2015-03-03
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using System;

namespace Unicorn.Reflection
{
    public static class Selection
    {
        public static GameObject activeGameObject
        {
            get => (GameObject)_GetActiveGameObject().GetValue(null, null);
            set => _GetActiveGameObject().SetValue(null, value, null);
        }

        private static System.Reflection.PropertyInfo _GetActiveGameObject()
        {
            return _activeGameObject ??= MyType.GetProperty("activeGameObject");
        }

        public static Transform activeTransform
        {
            get => (Transform)_GetActiveTransform().GetValue(null, null);
            set => _GetActiveTransform().SetValue(null, value, null);
        }

        private static System.Reflection.PropertyInfo _GetActiveTransform()
        {
            return _activeTransform ??= MyType.GetProperty("activeTransform");
        }

        private static System.Reflection.PropertyInfo _activeGameObject;
        private static System.Reflection.PropertyInfo _activeTransform;

        private static Type _myType;
        public static Type MyType => _myType ??= Type.GetType("UnityEditor.Selection,UnityEditor");
    }
}