/********************************************************************
created:    2025-07-21
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using UnityEngine.Events;

namespace Unicorn
{
    public static class ExtendedUnityEvent
    {
        public static Action On(this UnityEvent my, UnityAction fn)
        {
            if (my != null && fn != null)
            {
                my.AddListener(fn);
                return () => { my.RemoveListener(fn); };
            }

            return null;
        }

        public static Action On<T>(this UnityEvent<T> my, UnityAction<T> fn)
        {
            if (my != null && fn != null)
            {
                my.AddListener(fn);
                return () => { my.RemoveListener(fn); };
            }

            return null;
        }
    }
}