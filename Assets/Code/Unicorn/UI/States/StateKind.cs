/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn.UI.States
{
    internal enum StateKind
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