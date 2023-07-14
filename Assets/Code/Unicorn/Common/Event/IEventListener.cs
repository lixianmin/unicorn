/********************************************************************
created:    2023-07-14
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    public interface IEventListener
    {
        void AddListener(Action handler);
        void RemoveListener(Action handler);
    }
    
    public interface IEventListener<out T>
    {
        void AddListener(Action<T> handler);
        void RemoveListener(Action<T> handler);
    }
}