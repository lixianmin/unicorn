
/********************************************************************
created:    2018-03-19
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    public interface IRemoveListener
    {
        void RemoveListener (int message, Delegate listener);    
    }
}