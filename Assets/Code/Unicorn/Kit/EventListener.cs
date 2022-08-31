
/********************************************************************
created:    2022-08-26
author:     lixianmin

基于UIEventListener挂载的事件, 如果不主动调用RemoveAllListeners()释放回调方法的话, 会导致UIWindowBase对象无法正常释放
所以, 现在的办法是在UIButton的OnDestroy()方法中, 主动调用onClick事件的RemoveAllListeners()方法, 其它类后续效仿.

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Unicorn
{
    public class EventListener : Disposable
    {
        public void AddListener(UnityEvent evt, UnityAction handler)
        {
            if (evt != null && handler != null)
            {
                evt.AddListener(handler);
                _removeList.Add(() =>
                {
                    evt.RemoveListener(handler);
                });
            }
        }
        
        public void AddListener<T>(UnityEvent<T> evt, UnityAction<T> handler)
        {
            if (evt != null && handler != null)
            {
                evt.AddListener(handler);
                _removeList.Add(() =>
                {
                    evt.RemoveListener(handler);
                });
            }
        }

        public void RemoveAllListeners()
        {
            _removeList.InvokeAndClearEx();
        }

        protected override void _DoDispose(bool isManualDisposing)
        {
            RemoveAllListeners();
        }

        private readonly List<Action> _removeList = new(4);
    }
}