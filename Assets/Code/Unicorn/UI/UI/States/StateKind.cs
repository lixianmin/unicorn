/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn.UI.States
{
    internal enum StateKind : byte
    {
        None,
        Load,
        OpenAnimation,
        Opened,
        Unload,
        CloseAnimation,
        Closed,
        Failure,
    }
}