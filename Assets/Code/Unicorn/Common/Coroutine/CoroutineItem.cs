/********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.Collections;

namespace Unicorn
{
    public class CoroutineItem : IIsYieldable
    {
        internal CoroutineItem()
        {
        }

        public void Kill()
        {
            isKilled = true;
        }

        public override string ToString()
        {
            return $"routine={routine}, isDone={isDone}, isKilled={isKilled}, isRecyclable={isRecyclable}";
        }

        bool IIsYieldable.isYieldable => isDone || isKilled;

        internal IEnumerator routine;
        internal bool isRecyclable;

        public bool isDone { get; internal set; } // done normally.
        public bool isKilled { get; internal set; } // killed manually.
    }
}