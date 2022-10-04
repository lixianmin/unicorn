
/********************************************************************
created:    2022-08-15
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn.UI
{
    public enum RenderQueue : ushort
    {
        Background  = 0,
        Geometry    = 8000,
        Transparent = 16000,
        Overlay     = 24000,
    }
}