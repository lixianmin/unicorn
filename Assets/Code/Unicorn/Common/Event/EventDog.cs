/********************************************************************
created:    2022-08-26
author:     lixianmin

基于EventListener挂载的事件, 如果不主动调用RemoveAllListeners()释放回调方法的话, 会导致UIWindowBase对象无法正常释放
所以, 现在的办法是在UIButton的OnDestroy()方法中, 主动调用onClick事件的RemoveAllListeners()方法, 其它类后续效仿.

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using Unicorn.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Unicorn
{
    public class EventDog : Disposable
    {
        // Action不行, 因为它初始很可能是null, 因此必须使用ref; 然后使用ref后又无法用于lambda表达式中. 放弃, 直接使用UnityEvent吧
        // public void AddListener(Action evt, Action handler)
        // {
        //     if (evt != null && handler != null)
        //     {
        //         evt += handler;
        //         _removeList.Add(() => { evt -= handler; });
        //     }
        // }
        //
        // public void AddListener<T>(Action<T> evt, Action<T> handler)
        // {
        //     if (evt != null && handler != null)
        //     {
        //         evt += handler;
        //         _removeList.Add(() => { evt -= handler; });
        //     }
        // }

        public void AddListener(IEventListener listener, Action handler)
        {
            if (listener != null && handler != null)
            {
                listener.AddListener(handler);
                _removeList.Add(() => { listener.RemoveListener(handler); });
            }
        }

        public void AddListener<T>(IEventListener<T> listener, Action<T> handler)
        {
            if (listener != null && handler != null)
            {
                listener.AddListener(handler);
                _removeList.Add(() => { listener.RemoveListener(handler); });
            }
        }

        public void AddListener(UnityEvent evt, UnityAction handler)
        {
            if (evt != null && handler != null)
            {
                evt.AddListener(handler);
                _removeList.Add(() => { evt.RemoveListener(handler); });
            }
        }

        public void AddListener<T>(UnityEvent<T> evt, UnityAction<T> handler)
        {
            if (evt != null && handler != null)
            {
                evt.AddListener(handler);
                _removeList.Add(() => { evt.RemoveListener(handler); });
            }
        }

        public void AddListener<T1, T2>(UnityEvent<T1, T2> evt, UnityAction<T1, T2> handler)
        {
            if (evt != null && handler != null)
            {
                evt.AddListener(handler);
                _removeList.Add(() => { evt.RemoveListener(handler); });
            }
        }

        public void AddListener(EventTrigger trigger, EventTriggerType eventId, UnityAction<BaseEventData> handler)
        {
            if (trigger != null && handler != null)
            {
                var entry = new EventTrigger.Entry
                {
                    eventID = eventId
                };

                entry.callback.AddListener(handler);
                trigger.triggers.Add(entry);

                _removeList.Add(() => { trigger.triggers.Remove(entry); });
            }
        }

        public void AddRemoveHandler(Action handler)
        {
            if (handler != null)
            {
                _removeList.Add(handler);
            }
        }

        public void RemoveAllListeners()
        {
            _removeList.InvokeAndClear();
        }

        protected override void _DoDispose(int flags)
        {
            RemoveAllListeners();
        }

        private readonly Slice<Action> _removeList = new(4);
    }
}