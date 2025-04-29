
/********************************************************************
created:    2018-01-06
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using FieldInfo = System.Reflection.FieldInfo;

namespace Unicorn
{
    public static class ExtendedFieldInfo
    {
        static ExtendedFieldInfo ()
        {
            _Init_lpfnSetValueInternal();
        }

        public static void SetValueEx (this FieldInfo field, object target, object val)
        {
            if (null == field || null == target)
            {
                return;
            }

            _lpfnSetValueInternal(field, target, val);
        }

        private static void _Init_lpfnSetValueInternal ()
        {
            _lpfnSetValueInternal = null;

            var typeName = "System.Reflection.MonoField,mscorlib";
            var methodName = "SetValueInternal";

            var type = System.Type.GetType(typeName);
            if (null != type)
            {
                TypeTools.CreateDelegate(type, methodName, out _lpfnSetValueInternal);
            }

            if (null == _lpfnSetValueInternal)
            {
                _lpfnSetValueInternal = (field, target, val) => field.SetValue(target, val);
            }
        }

        private static Action<FieldInfo, object, object> _lpfnSetValueInternal;
    }
}