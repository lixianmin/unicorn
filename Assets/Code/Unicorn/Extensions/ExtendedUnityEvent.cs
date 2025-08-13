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
        /// <summary>
        /// 这个方法叫On(), 而不是At(). 一个原因是evt.后面输入O, 第一个就是On(), 而如果输入的是A, 第一个是AddListener()
        /// </summary>
        /// <param name="my"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
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
        
        public static Action On<T1, T2>(this UnityEvent<T1, T2> my, UnityAction<T1, T2> fn)
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