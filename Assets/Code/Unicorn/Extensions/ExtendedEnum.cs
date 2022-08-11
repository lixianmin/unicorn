
/********************************************************************
created:    2015-12-14
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    //[Obfuscators.ObfuscatorIgnore]
    internal static class ExtendedEnum
    {
		// http://www.codeproject.com/Tips/441086/NETs-Enum-HasFlag-and-performance-costs
//		public static bool HasFlag (this Enum that, Enum other)
//		{
//			var thatFlag  = Convert.ToUInt32(that);
//			var otherFlag = Convert.ToUInt32(other);
//			var has = (thatFlag & otherFlag) == otherFlag;
//			return has;
//		}
    }
}