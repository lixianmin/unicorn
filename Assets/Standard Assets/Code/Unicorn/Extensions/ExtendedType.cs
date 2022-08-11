
/*********************************************************************
created:    2014-12-17
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections;

namespace Unicorn
{
    public static class ExtendedType
    {
        internal static bool IsStaticClassEx (this Type type)
        {
            return null != type
                    && type.GetConstructor(Type.EmptyTypes) == null
                    && type.IsAbstract
                    && type.IsSealed;
        }

        public static string GetTypeNameEx (this Type type)
        {
            if (null == type)
            {
                return string.Empty;
            }

            var fullName = type.FullName;
            var className = !type.IsNested ? fullName : fullName.Replace('+', '.');
            return className;
        }
    }
}