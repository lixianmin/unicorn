
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
    class EnumLua: LuaBase
    {
		public override void WriteLoad (CodeWriter writer)
		{
			writer.WriteLine("item.{0} = aid.ReadInt32();", _name);
		}
    }
}