/********************************************************************
created:    2023-06-23
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn
{
    public static class ExtendedFloat
    {
        public static bool IsZero(this float my, float eps = 0.000001f)
        {
            return my < eps && my > -eps;
        }
    }
}