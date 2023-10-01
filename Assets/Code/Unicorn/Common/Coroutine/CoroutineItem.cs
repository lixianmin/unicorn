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
            Routine = null;
        }

        internal bool IsDoneOrKilled()
        {
            return (_flag & Flag.Done) != 0 || (_flag & Flag.Killed) != 0;
        }

        internal bool IsRecyclable()
        {
            return (_flag & Flag.Recyclable) != 0;
        }

        public override string ToString()
        {
            return $"routine={Routine}, IsDone={_HasFlag(Flag.Done)}, IsKilled={_HasFlag(Flag.Killed)}, " +
                   $"IsRecyclable={IsRecyclable()}";
        }

        private Flag _flag;
        internal IEnumerator Routine;
    }
}