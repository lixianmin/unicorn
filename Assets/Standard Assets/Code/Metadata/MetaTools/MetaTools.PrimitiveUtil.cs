

/********************************************************************
created:    2017-01-24
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using Unicorn.IO;

namespace Metadata
{
	partial class MetaTools
	{
		static class PrimitiveUtil
		{
			public static void Save (IOctetsWriter writer, object target, Type targetType)
			{
				if (targetType == typeof(Int32))
				{
					writer.Write((Int32) target);
				}
				else if (targetType == typeof(Single))
				{
					writer.Write((Single) target);
				}
				else if (targetType == typeof(Boolean))
				{
					writer.Write((Boolean) target);
				}
				else if (targetType == typeof(Int16))
				{
					writer.Write((Int16) target);
				}
				else if (targetType == typeof(Int64))
				{
					writer.Write((Int64) target);
				}
				else if (targetType == typeof(Byte))
				{
					writer.Write((Byte) target);
				}
				else if (targetType == typeof(Double))
				{
					writer.Write((Double) target);
				}
				else if (targetType == typeof(UInt16))
				{
					writer.Write((UInt16) target);
				}
				else if (targetType == typeof(UInt32))
				{
					writer.Write((UInt32) target);
				}
				else if (targetType == typeof(UInt64))
				{
					writer.Write((UInt64) target);
				}
				else if (targetType == typeof(SByte))
				{
					writer.Write((SByte) target);
				}
			}

            public static bool IsEqual (object lhsTarget, object rhsTarget, Type targetType)
            {
                if (targetType == typeof(Int32))
                {
                    var lhs = (Int32) lhsTarget;
                    var rhs = (Int32) rhsTarget;
                    return lhs == rhs;
                }
                else if (targetType == typeof(Single))
                {
                    var lhs = (Single) lhsTarget;
                    var rhs = (Single) rhsTarget;
                    var delta = lhs - rhs;

                    const float eps= 0.00001f;
                    return delta < eps && delta > -eps;
                }
                else if (targetType == typeof(Boolean))
                {
                    var lhs = (Boolean) lhsTarget;
                    var rhs = (Boolean) rhsTarget;
                    return lhs == rhs;
                }
                else if (targetType == typeof(Int16))
                {
                    var lhs = (Int16) lhsTarget;
                    var rhs = (Int16) rhsTarget;
                    return lhs == rhs;
                }
                else if (targetType == typeof(Int64))
                {
                    var lhs = (Int64) lhsTarget;
                    var rhs = (Int64) rhsTarget;
                    return lhs == rhs;
                }
                else if (targetType == typeof(Byte))
                {
                    var lhs = (Byte) lhsTarget;
                    var rhs = (Byte) rhsTarget;
                    return lhs == rhs;
                }
                else if (targetType == typeof(Double))
                {
                    var lhs = (Double) lhsTarget;
                    var rhs = (Double) rhsTarget;
                    var delta = lhs - rhs;

                    const double eps= 0.000001;
                    return delta < eps && delta > -eps;
                }
                else if (targetType == typeof(UInt16))
                {
                    var lhs = (UInt16) lhsTarget;
                    var rhs = (UInt16) rhsTarget;
                    return lhs == rhs;
                }
                else if (targetType == typeof(UInt32))
                {
                    var lhs = (UInt32) lhsTarget;
                    var rhs = (UInt32) rhsTarget;
                    return lhs == rhs;
                }
                else if (targetType == typeof(UInt64))
                {
                    var lhs = (UInt64) lhsTarget;
                    var rhs = (UInt64) rhsTarget;
                    return lhs == rhs;
                }
                else if (targetType == typeof(SByte))
                {
                    var lhs = (SByte) lhsTarget;
                    var rhs = (SByte) rhsTarget;
                    return lhs == rhs;
                }

                return false;
            }
		}
	}
}