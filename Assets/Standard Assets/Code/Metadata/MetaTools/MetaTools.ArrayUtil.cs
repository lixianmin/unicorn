

/********************************************************************
created:    2017-01-24
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections;
using Unicorn;
using Unicorn.IO;

namespace Metadata
{
	partial class MetaTools
	{
		static class ArrayUtil
		{
			public static Array Load (IOctetsReader reader, Type fieldType)
			{
                var basicType = (BasicType) reader.ReadByte();
                var count = reader.ReadInt32();

				var elementType = fieldType.GetElementType();
				var array = ArrayTools.CreateInstanceImpl(elementType, count);

				for (int i= 0; i< count; ++i)
				{
                    var val = _LoadObject(reader, elementType, basicType);
					array.SetValue(val, i);
				}

				return array;
			}

            public static void Save (IOctetsWriter writer, object target, bool isFullMode)
			{
				var array = target as Array;

                Type elementType = null != array ? array.GetElementTypeEx() : null;
                var basicType = null != elementType ? GetBasicType(elementType) : BasicType.Null;
                writer.Write((byte) basicType);

                var count = null != array ? array.GetLength(0) : 0;
                writer.Write(count);

				for (int i= 0; i< count; ++i)
				{
					var item = array.GetValue(i);
                    _SaveObject(writer, item, elementType, isFullMode);
				}
			}

            private static int _GetArrayLength (Array array)
            {
                if (null == array)
                {
                    return 0;
                }

                var length = array.GetLength(0);
                return length;
            }

            public static bool IsEqual (object lhsTarget, object rhsTarget)
            {
                var lhsArray = lhsTarget as Array;
                var rhsArray = rhsTarget as Array;

                var lhsCount = _GetArrayLength(lhsArray);
                var rhsCount = _GetArrayLength(rhsArray);

                if (lhsCount != rhsCount)
                {
                    return false;
                }

                if (lhsCount == 0)
                {
                    return true;
                }

                Type lhsElementType = lhsArray.GetElementTypeEx();
                Type rhsElementType = rhsArray.GetElementTypeEx();
                if (lhsElementType != rhsElementType)
                {
                    return false;
                }

                for (int i= 0; i< lhsCount; ++i)
                {
                    var lhsItem = lhsArray.GetValue(i);
                    var rhsItem = rhsArray.GetValue(i);

                    if (!_IsEqual(lhsItem, rhsItem, lhsElementType))
                    {
                        return false;
                    }
                }

                return true;
            }

//            public static IEnumerable EnumerateFields (object target, Type targetType)
//            {
//                var array = target as Array;
//                var elementType = targetType.GetElementType();
//                var count = null != array ? array.GetLength(0) : 0;
//
//                for (int i= 0; i< count; ++i)
//                {
//                    var item = array.GetValue(i);
//                    foreach (var ret in _EnumerateObject(item, elementType))
//                    {
//                        yield return ret;
//                    }
//                }
//            }
		}
	}
}