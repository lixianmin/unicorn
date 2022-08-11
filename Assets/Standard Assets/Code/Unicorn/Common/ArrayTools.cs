
/********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace Unicorn
{
    public static class ArrayTools
    {
        static ArrayTools()
        {
            _Init_lpfnCreateInstanceImpl();
        }

        public static int EnsureCapacity(int currentCapacity, int minCapacity)
        {
            var result = 0;

            if (currentCapacity <= 0)
            {
                result = 4;
            }
            else
            {
                const int maxIncrement = 256;
                if (currentCapacity <= maxIncrement)
                {
                    result = currentCapacity << 1;
                }
                else
                {
                    result = currentCapacity + maxIncrement;
                }
            }

            if (result < minCapacity)
            {
                result = minCapacity;
            }

            return result;
        }

        //		internal static void SetValueImpl (Array array, object val, int position)
        //		{
        //			if (null == array || position < 0)
        //			{
        //				return;
        //			}
        //
        //			var methodName = "SetValueImpl";
        //
        //			Action<object, int> lpfnSetValueImpl;
        //			TypeTools.CreateDelegate(array, methodName, out lpfnSetValueImpl);
        //
        //			if (null != lpfnSetValueImpl)
        //			{
        //				lpfnSetValueImpl(val, position);
        //			}
        //		}

        private static void _Init_lpfnCreateInstanceImpl()
        {
            _lpfnCreateInstanceImpl = null;
            _createInstanceImpl_lengths = new int[1];
            _createInstanceImpl_lowerBounds = new int[1];

            var typeName = "System.Array,mscorlib";
            var methodName = "CreateInstanceImpl";

            var type = System.Type.GetType(typeName);
            TypeTools.CreateDelegate(type, methodName, out _lpfnCreateInstanceImpl);

            if (null == _lpfnCreateInstanceImpl)
            {
                _lpfnCreateInstanceImpl = Array.CreateInstance;
            }
        }

        public static Array CreateInstanceImpl(Type elementType, int length)
        {
            _createInstanceImpl_lengths[0] = length;
            var array = _lpfnCreateInstanceImpl(elementType, _createInstanceImpl_lengths, _createInstanceImpl_lowerBounds);
            return array;
        }

        private static Func<Type, int[], int[], Array> _lpfnCreateInstanceImpl;
        private static int[] _createInstanceImpl_lengths;
        private static int[] _createInstanceImpl_lowerBounds;
    }
}