/********************************************************************
created:    2022-08-17
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Unicorn.UI
{
    public partial class UIWindowBase
    {
        private struct WidgetKey : IEquatable<WidgetKey>
        {
            public string name;
            public Type type;

            public override int GetHashCode()
            {
                return name.GetHashCode();
            }

            public bool Equals(WidgetKey right)
            {
                return name == right.name && type == right.type;
            }

            public override bool Equals(object right)
            {
                return null != right && Equals((WidgetKey)right);
            }

            public static bool operator ==(WidgetKey left, WidgetKey right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(WidgetKey left, WidgetKey right)
            {
                return !(left == right);
            }
        }

        public T GetWidget<T>(string name) where T : Component
        {
            return GetWidget(name, typeof(T)) as T;
        }

        public Component GetWidget(string name, Type type)
        {
            if (string.IsNullOrEmpty(name) || type == null)
            {
                return null;
            }

            var key = new WidgetKey { name = name, type = type };
            if (_widgets.TryGetValue(key, out var widget))
            {
                return widget;
            }

            widget = _transform.Dig(name, type);
            _widgets.Add(key, widget);
            return widget;
        }

        internal void _FillWidgets(UISerializer serializer)
        {
            if (serializer != null)
            {
                var dataList = serializer.widgets;
                if (dataList != null)
                {
                    foreach (var data in dataList)
                    {
                        // 在手机测试的时候, 这一部分有报NullReferenceException
                        if (data == null || data.name.IsNullOrEmpty() || data.target == null)
                        {
                            continue;
                        }

                        var key = new WidgetKey { name = data.name, type = data.target.GetType() };
                        _widgets.Add(key, data.target);
                    }
                }
            }
        }

        internal void _InitWidgetsWindow()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var topType = typeof(UIWindowBase);
            var type = GetType();

            // UISuit竟然继承的UIBag，这样循环调用一把就无法初始化UIBag中的成员变量了
            while (type != null && type != topType)
            {
                var fields = type.GetFields(flags);
                foreach (var field in fields)
                {
                    var fieldType = field.FieldType;
                    if (fieldType.IsSubclassOf(typeof(UIWidgetBase)))
                    {
                        var widget = field.GetValue(this) as UIWidgetBase;
                        widget?._SetWindow(this);
                    }
                }

                type = type.BaseType;
            }
        }

        private readonly Dictionary<WidgetKey, Component> _widgets = new(8);
    }
}