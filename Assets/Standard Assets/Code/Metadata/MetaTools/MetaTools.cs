
/********************************************************************
created:    2017-01-24
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using Unicorn;
using Unicorn.IO;
using System.Reflection;

namespace Metadata
{
    public static partial class MetaTools
	{
		public static bool IsMetadata (Type type)
		{
			return typeof(IMetadata).IsAssignableFrom(type) && !type.IsInterface;
		}

        internal static IMetadata Load (IOctetsReader reader, IMetadata metadata)
		{
            var hasMetadata = reader.ReadBoolean();
            if (!hasMetadata)
            {
                return null;
            }

            var typeName = reader.ReadString();
            var creator = MetaFactory.GetMetaCreator(typeName);
            if (null != creator)
            {
                metadata = metadata ?? creator.Create();
            }

			if (null != metadata)
			{
				var type = metadata.GetType();
				var flags = BindingFlags.Public | BindingFlags.Instance;
				var fields = TypeTools.GetSortedFields(type, flags);

				var fieldsCount = fields.Length;
                var layout = creator.GetLayout();

                try
                {
                    for (int i= 0; i< fieldsCount; ++i)
                    {
                        var field = fields[i];
                        var fieldType = field.FieldType;
                        var basicType = (BasicType) layout[i];
                        var val = _LoadObject(reader, fieldType, basicType);
                        field.SetValueEx(metadata, val);
                    }
                }
                catch (Exception ex)
                {
                    Logo.Error("[Load()] metadataType={0}, metadata={1}, ex={2}", typeName.ToString(), metadata, ex);
                }
			}

			return metadata;
		}

        private static object _LoadObject (IOctetsReader reader, Type fieldType, BasicType basicType)
		{
			switch (basicType)
			{
			case BasicType.Int32:
				return reader.ReadInt32();
			case BasicType.Single:
				return reader.ReadSingle();
			case BasicType.Boolean:
				return reader.ReadBoolean();
			case BasicType.Int16:
				return reader.ReadInt16();
            case BasicType.Int64:
				return reader.ReadInt64();
			case BasicType.Byte:
				return reader.ReadByte();
			case BasicType.Double:
				return reader.ReadDouble();
			case BasicType.UInt16:
				return reader.ReadUInt16();
			case BasicType.UInt32:
                return reader.ReadUInt32();
            case BasicType.Enum:
                return Enum.ToObject(fieldType, reader.ReadInt32());
			case BasicType.UInt64:
				return reader.ReadUInt64();
			case BasicType.SByte:
				return reader.ReadSByte();
			case BasicType.String:
				return reader.ReadString();
			case BasicType.Vector2:
                return UnityTypeAid.ReadVector2(reader);
			case BasicType.Vector3:
                return UnityTypeAid.ReadVector3(reader);
			case BasicType.Vector4:
                return UnityTypeAid.ReadVector4(reader);
			case BasicType.Color:
                return UnityTypeAid.ReadColor(reader);
			case BasicType.Array:
				return ArrayUtil.Load(reader, fieldType);
			case BasicType.List:
				return ListUtil.Load(reader, fieldType);
			case BasicType.LocaleText:
                return LocaleTextManager.It.ReadLocaleText(reader);
            case BasicType.VInt3:
                {
                    int x = reader.ReadInt32();
                    int y = reader.ReadInt32();
                    int z = reader.ReadInt32();
                    return new VInt3(x, y, z);
                }
            case BasicType.Int32_t:
                return (Int32_t) reader.ReadInt32();
            case BasicType.Float_t:
                return (Float_t) reader.ReadSingle();
            case BasicType.Int64_t:
                return (Int64_t) reader.ReadInt64();
			case BasicType.Metadata:
				return Load(reader, null);
			}

			return null;
		}

        internal static void Save (IOctetsWriter writer, IMetadata metadata, bool isFullMode)
		{
			if (null == writer)
			{
				return;
			}

            var hasMetadata = null != metadata;
            writer.Write(hasMetadata);

            if (!hasMetadata)
            {
                return;
            }

            var type = metadata.GetType();
            var metadataType = type.FullName;
			writer.Write(metadataType);

			var flags = BindingFlags.Public | BindingFlags.Instance;
			var fields = TypeTools.GetSortedFields(type, flags);

			var fieldsCount = fields.Length;
			for (int i= 0; i< fieldsCount; ++i)
			{
				var field = fields[i];
				var target = field.GetValue(metadata);
                _SaveObject(writer, target, field.FieldType, isFullMode);
			}
		}

        private static void _SaveObject (IOctetsWriter writer, object target, Type targetType, bool isFullMode)
		{
			if (targetType.IsPrimitive)
			{
				PrimitiveUtil.Save(writer, target, targetType);
			}
            else if (targetType == typeof(string))
			{
                var text = target as string;
                text = text ?? string.Empty;
				writer.Write(text);
			}
            else if (targetType == UnityTypeAid.TypeVector2)
			{
                UnityTypeAid.WriteVector2(writer, target);
			}
            else if (targetType == UnityTypeAid.TypeVector3)
			{
                UnityTypeAid.WriteVector3(writer, target);
			}
            else if (targetType == UnityTypeAid.TypeVector4)
			{
                UnityTypeAid.WriteVector4(writer, target);
			}
            else if (targetType == UnityTypeAid.TypeColor)
			{
                UnityTypeAid.WriteColor(writer, target);
			}
			else if (targetType.IsArray)
			{
                ArrayUtil.Save(writer, target, isFullMode);
			}
			else if (targetType.IsEnum)
			{
				writer.Write((int) target);
			}
			else if (targetType.IsGenericType)
			{
				var genericTypeDefinition = targetType.GetGenericTypeDefinition();

				if (typeof(List<>) == genericTypeDefinition)
				{
                    ListUtil.Save(writer, target, targetType, isFullMode);
				}
			}
			else if (targetType == typeof (LocaleText))
			{
				var text = (LocaleText) target;
                LocaleTextManager.It.WriteLocaleText(writer, text, isFullMode);
			}
            else if (targetType == typeof (VInt3))
            {
                var v = (VInt3) target;
                writer.Write(v.x);
                writer.Write(v.y);
                writer.Write(v.z);
            }
            else if (targetType == typeof (Int32_t))
            {
                writer.Write((Int32_t)target);
            }
            else if (targetType == typeof (Int64_t))
            {
                writer.Write((Int64_t)target);
            }
            else if (targetType == typeof (Float_t))
            {
                writer.Write((Float_t)target);
            }
			else if (IsMetadata(targetType))
			{
				var metadata = target as IMetadata;
                Save(writer, metadata, isFullMode);
			}
		}

        public static BasicType GetBasicType (Type targetType)
        {
            if (null == targetType)
            {
                return BasicType.Null;
            }

            if (targetType == typeof(Int32))
            {
                return BasicType.Int32;
            }
            else if (targetType == typeof(Single))
            {
                return BasicType.Single;
            }
            else if (targetType == typeof(Boolean))
            {
                return BasicType.Boolean;
            }
            else if (targetType == typeof(Int16))
            {
                return BasicType.Int16;
            }
            else if (targetType == typeof(Int64))
            {
                return BasicType.Int64;
            }
            else if (targetType == typeof(Byte))
            {
                return BasicType.Byte;
            }
            else if (targetType == typeof(Double))
            {
                return BasicType.Double;
            }
            else if (targetType == typeof(UInt16))
            {
                return BasicType.UInt16;
            }
            else if (targetType == typeof(UInt32))
            {
                return BasicType.UInt32;
            }
            else if (targetType == typeof(UInt64))
            {
                return BasicType.UInt64;
            }
            else if (targetType == typeof(SByte))
            {
                return BasicType.SByte;
            }
            else if (targetType == typeof(String))
            {
                return BasicType.String;
            }
            else if (targetType == UnityTypeAid.TypeVector2)
            {
                return BasicType.Vector2;
            }
            else if (targetType == UnityTypeAid.TypeVector3)
            {
                return BasicType.Vector3;
            }
            else if (targetType == UnityTypeAid.TypeVector4)
            {
                return BasicType.Vector4;
            }
            else if (targetType == UnityTypeAid.TypeColor)
            {
                return BasicType.Color;
            }
            else if (targetType.IsArray)
            {
                return BasicType.Array;
            }
            else if (targetType.IsEnum)
            {
                return BasicType.Enum;
            }
            else if (targetType.IsGenericType)
            {
                var genericTypeDefinition = targetType.GetGenericTypeDefinition();

                if (typeof(List<>) == genericTypeDefinition)
                {
                    return BasicType.List;
                }
            }
            else if (targetType == typeof (LocaleText))
            {
                return BasicType.LocaleText;
            }
            else if (targetType == typeof (VInt3))
            {
                return BasicType.VInt3;
            }
            else if (targetType == typeof (Int32_t))
            {
                return BasicType.Int32_t;
            }
            else if (targetType == typeof (Int64_t))
            {
                return BasicType.Int64_t;
            }
            else if (targetType == typeof (Float_t))
            {
                return BasicType.Float_t;
            }
            else if (IsMetadata(targetType))
            {
                return BasicType.Metadata;
            }

            throw new Exception("Unknown targetType= "+ targetType.ToString());
        }

        public static bool IsEqual (IMetadata lhsMetadata, IMetadata rhsMetadata)
        {
            if (null == lhsMetadata && null == rhsMetadata)
            {
                return true;
            }

            if (null == lhsMetadata || null == rhsMetadata)
            {
                return false;
            }

            var lhsType = lhsMetadata.GetType();
            var rhsType = rhsMetadata.GetType();
            if (lhsType != rhsType)
            {
                return false;
            }

            var flags = BindingFlags.Public | BindingFlags.Instance;
            var fields = TypeTools.GetSortedFields(lhsType, flags);

            var fieldsCount = fields.Length;
            for (int i= 0; i< fieldsCount; ++i)
            {
                var field = fields[i];
                var lhsTarget = field.GetValue(lhsMetadata);
                var rhsTarget = field.GetValue(rhsMetadata);

                if (!_IsEqual(lhsTarget, rhsTarget, field.FieldType))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool _IsEqual (object lhsTarget, object rhsTarget, Type targetType)
        {
            if (targetType.IsPrimitive)
            {
                return PrimitiveUtil.IsEqual(lhsTarget, rhsTarget, targetType);
            }
            else if (targetType == typeof(string))
            {
                var lhs = (string) lhsTarget;
                var rhs = (string) rhsTarget;

                // 这是因为XmlMetadata.Deserialize()中，空的string出来就是null，而我们自己加载的会是""
                if (string.IsNullOrEmpty(lhs) && string.IsNullOrEmpty(rhs))
                {
                    return true;
                }

                return lhs == rhs;
            }
            else if (targetType == UnityTypeAid.TypeVector2)
            {
                return UnityTypeAid.IsEqualVector2(lhsTarget, rhsTarget);
            }
            else if (targetType == UnityTypeAid.TypeVector3)
            {
                return UnityTypeAid.IsEqualVector3(lhsTarget, rhsTarget);
            }
            else if (targetType == UnityTypeAid.TypeVector4)
            {
                return UnityTypeAid.IsEqualVector4(lhsTarget, rhsTarget);
            }
            else if (targetType == UnityTypeAid.TypeColor)
            {
                return UnityTypeAid.IsEqualColor(lhsTarget, rhsTarget);
            }
            else if (targetType.IsArray)
            {
                return ArrayUtil.IsEqual(lhsTarget, rhsTarget);
            }
            else if (targetType.IsEnum)
            {
                var lhs = (Int32) lhsTarget;
                var rhs = (Int32) rhsTarget;
                return lhs == rhs;
            }
            else if (targetType.IsGenericType)
            {
                var genericTypeDefinition = targetType.GetGenericTypeDefinition();

                if (typeof(List<>) == genericTypeDefinition)
                {
                    return ListUtil.IsEqual(lhsTarget, rhsTarget);
                }
            }
            else if (targetType == typeof (LocaleText))
            {
                var lhs = (LocaleText) lhsTarget;
                var rhs = (LocaleText) rhsTarget;
                return lhs == rhs;
            }
            else if (targetType == typeof (VInt3))
            {
                var lhs = (VInt3) lhsTarget;
                var rhs = (VInt3) rhsTarget;
                return lhs == rhs;
            }
            else if (targetType == typeof (Int32_t))
            {
                var lhs = (Int32_t) lhsTarget;
                var rhs = (Int32_t) rhsTarget;
                return lhs == rhs;
            }
            else if (targetType == typeof (Int64_t))
            {
                var lhs = (Int64_t) lhsTarget;
                var rhs = (Int64_t) rhsTarget;
                return lhs == rhs;
            }
            else if (targetType == typeof (Float_t))
            {
                var lhs = (Float_t) lhsTarget;
                var rhs = (Float_t) rhsTarget;
                return lhs == rhs;
            }
            else if (IsMetadata(targetType))
            {
                var lhs = (IMetadata) lhsTarget;
                var rhs = (IMetadata) rhsTarget;
                return IsEqual(lhs, rhs);
            }

            return false;
        }

//        public static IEnumerable EnumerateFields (IMetadata metadata)
//        {
//            if (null != metadata)
//            {
//                var type = metadata.GetType();
//                var flags = BindingFlags.Public | BindingFlags.Instance;
//                var fields = TypeTools.GetSortedFields(type, flags);
//
//                var fieldsCount = fields.Length;
//                for (int i= 0; i< fieldsCount; ++i)
//                {
//                    var field = fields[i];
//                    var targetType = field.FieldType;
//                    var target = field.GetValue(metadata);
//
//                    foreach (var ret in _EnumerateObject(target, targetType))
//                    {
//                        yield return ret;
//                    }
//                }
//            }
//        }
//
//        private static IEnumerable _EnumerateObject (object target, Type targetType)
//        {
//            if (targetType.IsArray)
//            {
//                foreach (var ret in ArrayUtil.EnumerateFields(target, targetType))
//                {
//                    yield return ret;
//                }
//            }
//            else if (targetType.IsGenericType)
//            {
//                var genericTypeDefinition = targetType.GetGenericTypeDefinition();
//
//                if (typeof(List<>) == genericTypeDefinition)
//                {
//                    foreach (var ret in ListUtil.EnumerateFields(target, targetType))
//                    {
//                        yield return ret;
//                    }
//                }
//            }
//            else if (targetType == typeof (LocaleText))
//            {
//                yield return target;
//            }
//            else if (IsMetadata(targetType))
//            {
//                foreach (var ret in EnumerateFields(target as IMetadata))
//                {
//                    yield return ret;
//                }
//            }
//            else
//            {
//                yield return target;
//            }
//        }
	}
}