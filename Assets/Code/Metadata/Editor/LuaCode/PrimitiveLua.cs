
/********************************************************************
created:    2015-03-19
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using Unicorn;
using Unicorn.AutoCode;

namespace Metadata.LuaCode
{
    class PrimitiveLua: LuaBase
    {
		public override void WriteLoad (CodeWriter writer)
		{
			var getNameType = _GetAttributeValueRawType(_type) ?? _type;
			writer.WriteLine("item.{0} = aid.Read{1} ()", _name, getNameType.Name);
//			writer.WriteLine();
		}

        private static Type _GetAttributeValueRawType (Type type)
        {
            if (null != type)
            {
                if (type == typeof(Int32_t))
                {
                    return typeof(int);
                }
                else if (type == typeof(Int64_t))
                {
                    return typeof(long);
                }
                else if (type == typeof(Float_t))
                {
                    return typeof(float);
                }
            }

            return null;
        }
    }
}