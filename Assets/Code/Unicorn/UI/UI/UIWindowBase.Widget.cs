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
    public partial class  UIWindowBase
    {
        private struct WidgetKey :IEquatable<WidgetKey>
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
            
            public static bool operator == (WidgetKey left, WidgetKey right)
            {
                return left.Equals(right);
            }

            public static bool operator != (WidgetKey left, WidgetKey right)
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
            
            widget = _transform.DeepFindComponentEx(name, type);
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
                        var key = new WidgetKey { name = data.name, type = data.target.GetType() };
                        _widgets.Add(key, data.target);
                    }
                }    
            }
        }

        internal void _InitWidgetsWindow()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var fields = GetType().GetFields(flags);
            foreach (var field in fields)
            {
                var fieldType = field.FieldType;
                if (fieldType.IsSubclassOf(typeof(UIWidgetBase)))
                {
                    var widget = field.GetValue(this) as UIWidgetBase;
                    widget?._SetWindow(this);
                }
            }
        }

        private readonly Dictionary<WidgetKey, Component> _widgets = new(8);
    }
}