
/********************************************************************
created:    2022-08-15
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn.UI
{
    public enum RenderQueue : ushort
    {
        Background = 1000,
        Geometry = 2000,
        Transparent = 3000,
        Overlay = 4000,
    }
}