/********************************************************************
created:    2022-08-17
author:     lixianmin

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Unicorn.UI
{
    partial class UIWindowBase
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

        // public T GetWidget<T>(string name) where T : Component
        // {
        //     return GetWidget(name, typeof(T)) as T;
        // }

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
                        _widgets[key] = data.target;
                    }
                }
            }
        }

        /// <summary>
        /// 外部入口：初始化窗口的所有控件字段。
        /// 此方法会遍历当前窗口实例从子类到父类的继承链，
        /// 并对每一层声明的字段进行初始化。
        /// </summary>
        internal void _InitWidgetsWindowField()
        {
            // 定义需要查找的字段标志：实例的、公共的、非公共的。
            // 使用 BindingFlags.DeclaredOnly 来确保我们每次只获取在“当前类型”中声明的字段，
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                       BindingFlags.DeclaredOnly;

            // 直接以当前窗口实例 (this) 为起点，开始递归初始化。
            // 新的内部方法将处理好所有嵌套和继承的复杂情况。
            _InitWidgetsWindowFieldInner(this, flags);
        }

        /// <summary>
        /// 内部核心：递归地初始化一个对象及其内部所有嵌套视图中的控件。
        /// </summary>
        /// <param name="target">要进行字段初始化的目标对象实例。</param>
        /// <param name="flags">用于获取字段的绑定标志。</param>
        private void _InitWidgetsWindowFieldInner(object target, BindingFlags flags)
        {
            // 安全检查：如果目标对象为空，则没有字段可初始化，直接返回。
            if (target == null)
            {
                return;
            }

            // 获取目标对象的实际类型。
            var type = target.GetType();

            // 关键改动：我们在这里遍历当前对象“以及其所有父类”的继承链。
            // 您的旧代码是在外部使用 while 循环，现在我们将其整合到递归函数内部，
            // 使得这个函数对于任何对象（无论是主窗口还是嵌套View）都具有完整的初始化能力。
            while (type != null && type != typeof(object))
            {
                // 避免在遍历父类时重复处理子类的字段。
                var fields = type.GetFields(flags);

                foreach (var field in fields)
                {
                    // 获取字段在“当前目标对象”上的值（实例）。
                    var fieldValue = field.GetValue(target);

                    // 如果字段实例为null，则跳过。
                    if (fieldValue == null)
                    {
                        continue;
                    }

                    // 判断一：如果字段是一个UIWidgetBase控件。
                    // 使用 is 关键字既可以判断类型，又可以在判断成功后直接赋值给新变量 widget。
                    // 这是最现代、最高效、最安全的方式。
                    if (fieldValue is UIWidgetBase widget)
                    {
                        // 将最顶层的窗口实例 (this) 设置给它。
                        widget._SetWindow(this);
                    }
                    // 判断二：如果字段实现了IView接口（说明它是一个视图容器）。
                    // is 关键字同样完美适用于接口的判断。
                    else if (fieldValue is IView)
                    {
                        // *** 核心递归修正 ***
                        // 我们对这个字段的“实例” (fieldValue) 进行递归调用，
                        // 而不是它的类型。这样才能正确地初始化嵌套视图内部的控件。
                        _InitWidgetsWindowFieldInner(fieldValue, flags);
                    }
                }

                // 移动到父类型，继续循环，以处理父类中声明的字段。
                type = type.BaseType;
            }
        }

        private void _RemoveWidgetListeners()
        {
            if (_widgets.Count > 0)
            {
                foreach (var widget in _widgets.Values)
                {
                    if (widget is IRemoveAllListeners item)
                    {
                        item.RemoveAllListeners();
                    }
                }

                _widgets.Clear();
            }
        }

        // 所有的widget都是定义在UIWindow类中, 如果只是复用fetus的话, 每次都需要重新创建window对象, 那么这些_widgets的缓存放在fetus中也是没有意义的
        private readonly Dictionary<WidgetKey, Component> _widgets = new(8);
    }
}