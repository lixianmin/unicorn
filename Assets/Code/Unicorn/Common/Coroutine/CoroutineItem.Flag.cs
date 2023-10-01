/********************************************************************
created:    2023-10-01
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    partial class CoroutineItem
    {
        [Flags]
        internal enum Flag : byte
        {
            None = 0x00,
            Done = 0x01,
            Killed = 0x02,
            Recyclable = 0x04,
        }

        internal void AddFlag(Flag flag)
        {
            _flag |= flag;
        }

        internal void RemoveFlag(Flag flag)
        {
            _flag &= ~flag;
        }

        private bool _HasFlag(Flag flag)
        {
            return (_flag & flag) != 0;
        }
    }
}