/********************************************************************
created:    2022-09-09
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    public class RangeFloatAttribute : Attribute
    {
        public RangeFloatAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
        
        public float Min { get; private set; }
        public float Max { get; private set; }
    }
}