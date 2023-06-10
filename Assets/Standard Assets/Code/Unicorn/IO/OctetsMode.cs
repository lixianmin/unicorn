
/********************************************************************
created:    2015-09-09
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn.IO
{
    [Flags]
    public enum OctetsMode: ushort
    {
        None        = 0,
        UseFilter   = 1,
		Compress	= 2,
    }
}
