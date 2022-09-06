
/********************************************************************
created:    2022-09-06
author:     lixianmin

purpose:    assert
Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;

namespace Unicorn
{
    public static partial class TypeTools
    {
        public static int SetDefaultTypeIndex(Type type)
        {
            if (_typeIndices.TryGetValue(type, out var index))
            {
                return index;
            }
            
            index = ++_indexGenerator;
            _typeIndices[type] = index;

            return index;
        }

        private static int _indexGenerator;
        private static readonly Dictionary<Type, int> _typeIndices = new();
    }
}