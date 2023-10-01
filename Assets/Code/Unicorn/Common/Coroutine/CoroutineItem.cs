/********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;

namespace Unicorn
{
    public partial class CoroutineItem
    {
        internal CoroutineItem()
        {
        }

        public void Kill()
        {
            _flag |= Flag.Killed;
        }

        public override string ToString()
        {
            return $"routine={routine}, isDone={isDone}, isKilled={isKilled}, isRecyclable={isRecyclable}";
        }

        private Flag _flag;
        internal IEnumerator routine;

        public bool isDone => HasFlag(Flag.Done); // done normally.
        public bool isKilled => HasFlag(Flag.Killed); // killed manually.

        internal bool isRecyclable => HasFlag(Flag.Recyclable);
    }
}