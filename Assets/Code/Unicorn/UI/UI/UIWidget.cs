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
        /// 获取具体的UI对象, 用于注册事件或设置UI属性
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public T UI
        {
            get
            {
                if (_widget is null)
                {
                    if (_window is null)
                    {
                        throw new NullReferenceException("1.临时变量: 创建时未传window参数 2. 类成员变量: 资源未OnLoaded()");
                    }
                    
                    _widget = _window.GetWidget(_name, typeof(T)) as T;
                    if (_widget == null)
                    {
                        throw new NullReferenceException($"can not find the _widget with _name={_name}");
                    }
                }

                return _widget;
            }
        }

        private T _widget;
    }
}