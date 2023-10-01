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

        internal void Reset()
        {
            _flag = Flag.None;
            routine = null;
        }

        public override string ToString()
        {
            return $"routine={routine}, isDone={isDone}, isKilled={isKilled}, isRecyclable={isRecyclable}";
        }

        private Flag _flag;
        internal IEnumerator routine;

        public bool isDone => _HasFlag(Flag.Done); // done normally.
        public bool isKilled => _HasFlag(Flag.Killed); // killed manually.

        internal bool isRecyclable => _HasFlag(Flag.Recyclable);
    }
}