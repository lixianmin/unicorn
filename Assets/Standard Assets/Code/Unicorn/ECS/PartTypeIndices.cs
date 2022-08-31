
/********************************************************************
created:    2018-04-03
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;

namespace Unicorn
{
    public static class PartTypeIndices
    {
        public static int SetDefaultTypeIndex(Type type)
        {
            if (!_typeIndices.TryGetValue(type, out var typeIndex))
            {
                typeIndex = ++_typeGenerator;
                _typeIndices[type] = typeIndex;
            }

            return typeIndex;
        }

        private static int _typeGenerator;
        private static readonly Dictionary<Type, int> _typeIndices = new();
    }
}