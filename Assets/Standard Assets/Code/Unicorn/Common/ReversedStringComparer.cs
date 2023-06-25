
/*********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections.Generic;

namespace Unicorn
{
    public class ReversedStringComparer : IComparer<string>
    {
        static ReversedStringComparer()
        {
        }

        private ReversedStringComparer()
        {
        }

        public int Compare(string a, string b)
        {
            return a.ReversedCompareTo(b);
        }

        public static readonly ReversedStringComparer Instance = new ReversedStringComparer();
    }
}