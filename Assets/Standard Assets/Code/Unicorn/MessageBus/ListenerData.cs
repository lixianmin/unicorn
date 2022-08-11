
/********************************************************************
created:    2018-03-19
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    public struct ListenerData
    {
        public IRemoveListener sender;
        public int message;
        public Delegate listener;

        public void RemoveListener ()
        {
            if (null != sender && null != listener)
            {
                sender.RemoveListener(message, listener);
            }
        }
    }
}