/*********************************************************************
created:    2014-12-17
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    public static class TypeEx
    {
        internal static bool IsStaticClass(this Type type)
        {
            return null != type
                   && type.GetConstructor(Type.EmptyTypes) == null
                   && type.IsAbstract
                   && type.IsSealed;
        }

        public static string GetTypeName(this Type type)
        {
            if (null == type)
            {
                return string.Empty;
            }

            var fullName = type.FullName ?? string.Empty;
            var className = !type.IsNested ? fullName : fullName.Replace('+', '.');
            return className;
        }
    }
}