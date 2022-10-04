
/********************************************************************
created:    2022-08-15
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn.UI
{
    public enum RenderQueue : ushort
    {
        Background  = 5000,
        Geometry    = 10000,
        Transparent = 15000,
        Overlay     = 20000,
    }
}