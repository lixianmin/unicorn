

/********************************************************************
created:    2017-06-27
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections.Generic;
using Unicorn.IO;
using UnityEngine;

namespace Metadata
{
	public static class LuaLoadAid
	{
        static LuaLoadAid ()
        {
            InnerUnityTypeAid.Init();
            _aid = MetadataManager.Instance.GetLoadAid();
        }

        public static bool Seek (string metadataType, int metadataId)
        {
            return _aid.Seek(metadataType, metadataId);
        }

        public static IEnumerable<int> EnumerateIDs (string metadataType)
        {
            return _aid.EnumerateIDs(metadataType);
        }

        public static VInt3 ReadVInt3 ()
        {
            int x = _aid.ReadInt32();
            int y = _aid.ReadInt32();
            int z = _aid.ReadInt32();
            var result = new VInt3(x, y, z);
            return result;
        }

        public static string    ReadLocaleText ()   { return LocaleTextManager.Instance.ReadLocaleText(_aid); }

        public static string    ReadString ()   { return _aid.ReadString(); }

        public static bool      ReadBoolean ()  { return _aid.ReadBoolean(); }

        public static byte      ReadByte ()     { return _aid.ReadByte(); }

        public static sbyte     ReadSByte ()    { return _aid.ReadSByte(); }

        public static short     ReadInt16 ()    { return _aid.ReadInt16(); }

        public static ushort    ReadUInt16 ()   { return _aid.ReadUInt16(); }

        public static int       ReadInt32 ()    { return _aid.ReadInt32(); }

        public static uint      ReadUInt32 ()   { return _aid.ReadUInt32(); }

        public static long      ReadInt64 ()    { return _aid.ReadInt64(); }

        public static ulong     ReadUInt64 ()   { return _aid.ReadUInt64(); }

        public static float     ReadSingle ()   { return _aid.ReadSingle(); }

        public static double    ReadDouble ()   { return _aid.ReadDouble(); }

        public static Color     ReadColor ()    { return _aid.ReadColor(); }

        public static Vector2   ReadVector2 ()  { return _aid.ReadVector2(); }

        public static Vector3   ReadVector3 ()  { return _aid.ReadVector3(); }

        public static Vector4   ReadVector4 ()  { return _aid.ReadVector4(); }

        private static LoadAid _aid;
	}
}