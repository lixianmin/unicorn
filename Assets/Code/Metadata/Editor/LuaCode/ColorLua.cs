
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
    class ColorLua: LuaBase
    {
		public override void WriteLoad (CodeWriter writer)
		{
			writer.WriteLine("item.{0} = aid.ReadColor()", _name);
//            writer.WriteLine();
		}
    }
}