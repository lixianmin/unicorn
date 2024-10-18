
/********************************************************************
created:    2018-03-14
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Reflection;

namespace Unicorn
{
    public partial class EntityBase
    {
        public IPart AddPart(Type type)
        {
            if (type != null)
            {
                _parts ??= new EntityTable();
                return _AddPart(type, true);
            }

            return null;
        }

        private IPart _AddPart(Type type, bool checkDuplicated)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var constructor = type.GetConstructor(flags, null, CallingConventions.Any, Array.Empty<Type>(), null);
            var part = constructor?.Invoke(Array.Empty<object>()) as IPart;
            if (part != null)
            {
                if (part is IInitPart initPart)
                {
                    initPart.InitPart(this);
                }

                _parts.Add(type, part, checkDuplicated);

                OnPartCreated?.Invoke(part);
            }

            return part;
        }

        public IPart GetPart(Type type)
        {
            var parts = _parts;
            if (type != null && parts != null)
            {
                return parts.GetPart(type);
            }

            return null;
        }

        public IPart SetDefaultPart(Type type)
        {
            if (type != null)
            {
                _parts ??= new EntityTable();
                var part = _parts.GetPart(type) ?? _AddPart(type, false);

                return part;
            }

            return null;
        }

        public bool RemovePart(Type type)
        {
            var parts = _parts;
            if (type != null && parts != null)
            {
                return parts.Remove(type);
            }

            return false;
        }

        /// <summary>
        /// 清理Entity状态.
        /// Entity原本实现了Dispose()方法, 但目前评估加一个Clear()方法可能更合理. 原因是:
        /// 1. Dispose()方法是销毁对象, 但之后对象就无法复用了. 对比之下Clear()方法代表对象还可以接着使用, Clear()方法优势更大
        /// 2. 有些类一开始并未做Ecs设计, 因此Entity只能后续作为作为其类的成员对象出现, 则有自动Dispose()的需求. 
        /// 3. 然而, Disposeable是在主线程回收的, 目前无法支持到Unicorn.Core
        /// </summary>
        public virtual void Clear()
        {
            if (_parts != null)
            {
                _parts.Dispose();
                _parts = null;
            }
        }

        private EntityTable _parts;

        // global variables
        public static event Action<IPart> OnPartCreated;
    }
}