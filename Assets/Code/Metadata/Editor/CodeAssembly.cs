
/********************************************************************
created:    2015-06-21
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;

namespace Metadata
{
	public enum CodeAssembly
	{
		None,
		StandardAssembly= 1,
		ClientAssembly	= 2,
		EditorAssembly	= 4,
		All = StandardAssembly + ClientAssembly + EditorAssembly,
	}
}