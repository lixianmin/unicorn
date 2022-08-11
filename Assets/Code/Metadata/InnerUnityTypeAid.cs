

/********************************************************************
created:    2017-06-27
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
#pragma warning disable 0414
using System;
using Unicorn.IO;
using UnityEngine;

namespace Metadata
{
    internal static class InnerUnityTypeAid
	{
        // We use this method to withstand Unity compile stripping.
        internal static void Init ()
        {
            _readers = new Func<IOctetsReader, object>[] { ReadColor, ReadVector2, ReadVector3, ReadVector4 };
            _writers = new Action<IOctetsWriter, object>[] {WriteColor, WriteVector2, WriteVector3, WriteVector4 };
        }

        public static object ReadColor (IOctetsReader reader)    { return reader.ReadColor(); }

        public static object ReadVector2 (IOctetsReader reader)  { return reader.ReadVector2(); }

        public static object ReadVector3 (IOctetsReader reader)  { return reader.ReadVector3(); }

        public static object ReadVector4 (IOctetsReader reader)  { return reader.ReadVector4(); }

        public static void WriteColor (IOctetsWriter writer, object color)
        {
            writer.Write((Color) color);
        }

        public static void WriteVector2 (IOctetsWriter writer, object v)
        {
            writer.Write((Vector2) v);
        }

        public static void WriteVector3 (IOctetsWriter writer, object v)
        {
            writer.Write((Vector3) v);
        }

        public static void WriteVector4 (IOctetsWriter writer, object v)
        {
            writer.Write((Vector4) v);
        }

        public static bool IsEqualVector2 (object lhs, object rhs)
        {
            return (Vector2) lhs == (Vector2) rhs;
        }

        public static bool IsEqualVector3 (object lhs, object rhs)
        {
            return (Vector3) lhs == (Vector3) rhs;
        }

        public static bool IsEqualVector4 (object lhs, object rhs)
        {
            return (Vector4) lhs == (Vector4) rhs;
        }

        public static bool IsEqualColor (object lhs, object rhs)
        {
            var lhsColor = (Color) lhs;
            var rhsColor = (Color) rhs;

            return _IsEqualColor(lhsColor.r, rhsColor.r)
                && _IsEqualColor(lhsColor.g, rhsColor.g)
                && _IsEqualColor(lhsColor.b, rhsColor.b)
                && _IsEqualColor(lhsColor.a, rhsColor.a);
        }

        private static bool _IsEqualColor (float lhs, float rhs)
        {
            var eps = 0.04f;
            var delta = lhs - rhs;
            var ret = delta < eps && delta > -eps;
            return ret;
        }

        private static Func<IOctetsReader, object>[] _readers;
        private static Action<IOctetsWriter, object>[] _writers;
	}
}