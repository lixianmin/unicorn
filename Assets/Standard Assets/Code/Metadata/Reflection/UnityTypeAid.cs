
/********************************************************************
created:    2017-06-28
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using Unicorn;
using Unicorn.IO;

namespace Metadata
{
    internal static class UnityTypeAid
    {
        static UnityTypeAid ()
        {
            _innerUnityTypeAidType = TypeTools.SearchType("Metadata.InnerUnityTypeAid");

            if (null != _innerUnityTypeAidType)
            {
                TypeTools.CreateDelegate(_innerUnityTypeAidType, "ReadColor", out _lpfnReadColor);
                TypeTools.CreateDelegate(_innerUnityTypeAidType, "ReadVector2", out _lpfnReadVector2);
                TypeTools.CreateDelegate(_innerUnityTypeAidType, "ReadVector3", out _lpfnReadVector3);
                TypeTools.CreateDelegate(_innerUnityTypeAidType, "ReadVector4", out _lpfnReadVector4);
            }
        }

        private static Func<IOctetsReader, object> _lpfnReadColor;
        public static object ReadColor (IOctetsReader reader)
        {
            if (null != _lpfnReadColor)
            {
                return _lpfnReadColor(reader);
            }

            return null;
        }

        private static Func<IOctetsReader, object> _lpfnReadVector2;
        public static object ReadVector2 (IOctetsReader reader)
        {
            if (null != _lpfnReadVector2)
            {
                return _lpfnReadVector2(reader); 
            }

            return null;
        }

        private static Func<IOctetsReader, object> _lpfnReadVector3;
        public static object ReadVector3 (IOctetsReader reader)
        {
            if (null != _lpfnReadVector3)
            {
                return _lpfnReadVector3(reader); 
            }

            return null;
        }

        private static Func<IOctetsReader, object> _lpfnReadVector4;
        public static object ReadVector4 (IOctetsReader reader)
        {
            if (null != _lpfnReadVector4)
            {
                return _lpfnReadVector4(reader); 
            }

            return null;
        }

        private static Action<IOctetsWriter, object> _lpfnWriteColor;
        public static void WriteColor (IOctetsWriter writer, object color)
        {
            if (null == _lpfnWriteColor)
            {
                TypeTools.CreateDelegate(_innerUnityTypeAidType, "WriteColor", out _lpfnWriteColor);
            }

            if (null != _lpfnWriteColor)
            {
                _lpfnWriteColor(writer, color);
            }
        }

        private static Action<IOctetsWriter, object> _lpfnWriteVector2;
        public static void WriteVector2 (IOctetsWriter writer, object v)
        {
            if (null == _lpfnWriteVector2)
            {
                TypeTools.CreateDelegate(_innerUnityTypeAidType, "WriteVector2", out _lpfnWriteVector2);
            }

            if (null != _lpfnWriteVector2)
            {
                _lpfnWriteVector2(writer, v);
            }
        }

        private static Action<IOctetsWriter, object> _lpfnWriteVector3;
        public static void WriteVector3 (IOctetsWriter writer, object v)
        {
            if (null == _lpfnWriteVector3)
            {
                TypeTools.CreateDelegate(_innerUnityTypeAidType, "WriteVector3", out _lpfnWriteVector3);
            }

            if (null != _lpfnWriteVector3)
            {
                _lpfnWriteVector3(writer, v);
            }
        }

        private static Action<IOctetsWriter, object> _lpfnWriteVector4;
        public static void WriteVector4 (IOctetsWriter writer, object v)
        {
            if (null == _lpfnWriteVector4)
            {
                TypeTools.CreateDelegate(_innerUnityTypeAidType, "WriteVector4", out _lpfnWriteVector4);
            }

            if (null != _lpfnWriteVector4)
            {
                _lpfnWriteVector4(writer, v);
            }
        }

        private static Func<object, object, bool> _lpfnIsEqualVector2;
        public static bool IsEqualVector2 (object lhs, object rhs)
        {
            if (null == _lpfnIsEqualVector2)
            {
                TypeTools.CreateDelegate(_innerUnityTypeAidType, "IsEqualVector2", out _lpfnIsEqualVector2);
            }

            return null != _lpfnIsEqualVector2 && _lpfnIsEqualVector2(lhs, rhs);
        }

        private static Func<object, object, bool> _lpfnIsEqualVector3;
        public static bool IsEqualVector3 (object lhs, object rhs)
        {
            if (null == _lpfnIsEqualVector3)
            {
                TypeTools.CreateDelegate(_innerUnityTypeAidType, "IsEqualVector3", out _lpfnIsEqualVector3);
            }

            return null != _lpfnIsEqualVector3 && _lpfnIsEqualVector3(lhs, rhs);
        }

        private static Func<object, object, bool> _lpfnIsEqualVector4;
        public static bool IsEqualVector4 (object lhs, object rhs)
        {
            if (null == _lpfnIsEqualVector4)
            {
                TypeTools.CreateDelegate(_innerUnityTypeAidType, "IsEqualVector4", out _lpfnIsEqualVector4);
            }

            return null != _lpfnIsEqualVector4 && _lpfnIsEqualVector4(lhs, rhs);
        }

        private static Func<object, object, bool> _lpfnIsEqualColor;
        public static bool IsEqualColor (object lhs, object rhs)
        {
            if (null == _lpfnIsEqualColor)
            {
                TypeTools.CreateDelegate(_innerUnityTypeAidType, "IsEqualColor", out _lpfnIsEqualColor);
            }

            return null != _lpfnIsEqualColor && _lpfnIsEqualColor(lhs, rhs);
        }

        private static Type _typeColor;
        public static Type TypeColor
        {
            get
            {
                if (null == _typeColor)
                {
                    _typeColor = Type.GetType("UnityEngine.Color,UnityEngine");
                }

                return _typeColor;
            }
        }

        private static Type _typeVector2;
        public static Type TypeVector2
        {
            get
            {
                if (null == _typeVector2)
                {
                    _typeVector2 = Type.GetType("UnityEngine.Vector2,UnityEngine");
                }

                return _typeVector2;
            }
        }

        private static Type _typeVector3;
        public static Type TypeVector3
        {
            get
            {
                if (null == _typeVector3)
                {
                    _typeVector3 = Type.GetType("UnityEngine.Vector3,UnityEngine");
                }

                return _typeVector3;
            }
        }

        private static Type _typeVector4;
        public static Type TypeVector4
        {
            get
            {
                if (null == _typeVector4)
                {
                    _typeVector4 = Type.GetType("UnityEngine.Vector4,UnityEngine");
                }

                return _typeVector4;
            }
        }

        private static Type _innerUnityTypeAidType;
    }
}
