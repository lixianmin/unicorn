/********************************************************************
created:    2022-08-17
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using UnityEngine;

namespace Unicorn.UI
{
    public abstract class UIWidgetBase
    {
        internal void _SetWindow(UIWindowBase window)
        {
            _window = window;
            // Logo.Warn($"fetus={fetus} name={_name}");
        }

        public string GetName()
        {
            return _name;
        }

        protected UIWindowBase _window;
        protected string _name;
    }

    public class UIWidget<T> : UIWidgetBase where T : Component
    {
        /// <summary>
        /// 创建UIWidget对象
        /// </summary>
        /// <param name="name">一定不为null或""</param>
        /// <param name="window">如果UIWidget是UIWindowBase的类成员变量则填null, 由UI库自动填充. 如果UIWidget是临时变量, 则必须填充window参数</param>
        public UIWidget(string name, UIWindowBase window = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name is null or empty");
            }

            _name = name;
            _window = window;
        }

        /// <summary>
        /// 获取具体的UI组件. 获取不到的时候只打印日志, 不再抛出异常, 这样client有机会通过判空绕过异常
        /// </summary>
        public T UI
        {
            get
            {
                if (_widget is null)
                {
                    if (_window is null)
                    {
                        Logo.Warn("1.临时变量: 创建时未传window参数 2. 类成员变量: 资源未OnLoaded()");
                        return null;
                    }

                    _widget = _window.GetWidget(_name, typeof(T)) as T;
                    if (_widget == null)
                    {
                        Logo.Warn("can not find the _widget with _name={0}", _name);
                        _widget = null;
                    }
                }

                return _widget;
            }
        }

        private T _widget;
    }
}