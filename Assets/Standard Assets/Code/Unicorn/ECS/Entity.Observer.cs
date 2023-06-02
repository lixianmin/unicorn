
/********************************************************************
created:    2018-03-14
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using Unicorn.Collections;

namespace Unicorn
{
    partial class Entity
    {
        class Observer
        {
            public void SendMessage (int message)
            {
                var processor = _GetProcessor(message);
                if (null != processor)
                {
                    try
                    {
                        processor();
                    }
                    catch (Exception ex)
                    {
                        Logo.Error("[SendMessage()] message=[{0}], ex={1}", message.ToString(), ex);
                    }
                }
            }

            public void AddListener (int message, Action listener)
            {
                if (null == listener)
                {
                    return;
                }

                var processor = _GetProcessor(message);
                if (null != processor)
                {
                    processor += listener;
                }
                else
                {
                    processor = listener;
                }

                _processors[message] = processor;
            }

            public void RemoveListener (int message, Action listener)
            {
                if (null == listener)
                {
                    return;
                }

                var processor = _GetProcessor(message);
                if (null != processor)
                {
                    processor -= listener;
                    if (null != processor)
                    {
                        _processors[message] = processor;
                    }
                    else
                    {
                        _processors.Remove(message);
                    }
                }
            }

            public void RemoveAllListeners ()
            {
                if (_processors.Count > 0)
                {
                    _processors.Clear();
                }
            }

            private Action _GetProcessor (int message)
            {
                if (_processors.TryGetValue(message, out var target))
                {
                    var processor = target as Action;
                    return processor;
                }

                return null;
            }

            private readonly SortedTable<int, object> _processors = new();
        }
    }
}