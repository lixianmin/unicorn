/********************************************************************
created:    2023-06-23
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn
{
    public static class FloatEx
    {
        public static bool IsZero(this float my, float eps = Eps)
        {
            return my < eps && my > -eps;
        }

        public static bool IsEqual(this float my, float other, float eps = Eps)
        {
            var delta = my - other;
            return IsZero(delta, eps);
        }

        private const float Eps = 0.000001f;
    }
}