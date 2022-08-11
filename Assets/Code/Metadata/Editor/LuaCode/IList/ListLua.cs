
/********************************************************************
created:    2015-03-19
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Unicorn;

namespace Metadata.LuaCode
{
    class ListLua: IListLua
    {
		protected override Type _GetElementType ()
		{
			var elementType = _type.GetGenericArguments() [0];
			return elementType;
		}
    }
}