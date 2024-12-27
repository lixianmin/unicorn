/********************************************************************
created:    2015-03-03
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn.Reflection
{
    public static class EditorPrefs
    {
        public static int GetInt(string key, int defaultValue = 0)
        {
            if (null == _lpfnGetInt)
            {
                var method = MyType.GetMethod("GetInt", new[] { typeof(string), typeof(int) });
                TypeTools.CreateDelegate(method, out _lpfnGetInt);
            }

            var result = _lpfnGetInt(key, defaultValue);
            return result;
        }

        public static void SetInt(string key, int value)
        {
            if (null == _lpfnSetInt)
            {
                var method = MyType.GetMethod("SetInt", _GetMethodFlags);
                TypeTools.CreateDelegate(method, out _lpfnSetInt);
            }

            _lpfnSetInt(key, value);
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            if (null == _lpfnGetBool)
            {
                var method = MyType.GetMethod("GetBool", new[] { typeof(string), typeof(bool) });
                TypeTools.CreateDelegate(method, out _lpfnGetBool);
            }

            var result = _lpfnGetBool(key, defaultValue);
            return result;
        }

        public static void SetBool(string key, bool value)
        {
            if (null == _lpfnSetBool)
            {
                var method = MyType.GetMethod("SetBool", _GetMethodFlags);
                TypeTools.CreateDelegate(method, out _lpfnSetBool);
            }

            _lpfnSetBool(key, value);
        }

        public static string GetString(string key, string defaultValue = "")
        {
            if (null == _lpfnGetString)
            {
                var method = MyType.GetMethod("GetString", new[] { typeof(string), typeof(string) });
                TypeTools.CreateDelegate(method, out _lpfnGetString);
            }

            var result = _lpfnGetString(key, defaultValue);
            return result;
        }

        public static void SetString(string key, string value)
        {
            if (null == _lpfnSetString)
            {
                var method = MyType.GetMethod("SetString",
                    _GetMethodFlags);
                TypeTools.CreateDelegate(method, out _lpfnSetString);
            }

            _lpfnSetString(key, value);
        }

        public static float GetFloat(string key, float defaultValue = 0f)
        {
            if (null == _lpfnGetFloat)
            {
                var method = MyType.GetMethod("GetFloat", new[] { typeof(string), typeof(float) });
                TypeTools.CreateDelegate(method, out _lpfnGetFloat);
            }

            var result = _lpfnGetFloat(key, defaultValue);
            return result;
        }

        public static void SetFloat(string key, float value)
        {
            if (null == _lpfnSetFloat)
            {
                var method = MyType.GetMethod("SetFloat", _GetMethodFlags);
                TypeTools.CreateDelegate(method, out _lpfnSetFloat);
            }

            _lpfnSetFloat(key, value);
        }

        private static Func<string, int, int> _lpfnGetInt;
        private static Action<string, int> _lpfnSetInt;

        private static Func<string, bool, bool> _lpfnGetBool;
        private static Action<string, bool> _lpfnSetBool;

        private static Func<string, string, string> _lpfnGetString;
        private static Action<string, string> _lpfnSetString;

        private static Func<string, float, float> _lpfnGetFloat;
        private static Action<string, float> _lpfnSetFloat;

        private static readonly System.Reflection.BindingFlags _GetMethodFlags =
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static;

        private static Type _myType;
        public static Type MyType => _myType ??= Type.GetType("UnityEditor.EditorPrefs,UnityEditor");
    }
}