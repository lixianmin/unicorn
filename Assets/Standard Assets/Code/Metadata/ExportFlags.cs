
/********************************************************************
created:    2015-03-19
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;

namespace Metadata
{
    [Flags]
    public enum ExportFlags: ushort
    {
		None			= 0x00,
		ExportRaw		= 0x01,
		ExportLua		= 0x02,

//		KeepToString	= 0x04,
//		AutoCode		= 0x08,

		IncrementBuild	= 0x10,
		ClearBuiltFile	= 0x20,

		ExportAll		= ExportRaw | ExportLua
    }
}