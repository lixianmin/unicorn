
/********************************************************************
created:    2015-03-19
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;
using Unicorn;

namespace Metadata.LuaCode
{
	class ArrayLua: IListLua
    {
		protected override Type _GetElementType ()
		{
			var elementType = _type.GetElementType();
			return elementType;
		}
    }
}