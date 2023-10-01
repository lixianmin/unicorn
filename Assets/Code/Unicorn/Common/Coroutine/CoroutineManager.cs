/********************************************************************
created:    2022-08-11
author:     lixianmin

1. Coroutine using is strictly limited in our game.

2. RoutineManger supports "yield return null" usage, and IIsYieldable implemented classes, while "yield return www" is not supported.

3. We can not use TickAmortize to amortize coroutine time, because several coroutines may need to be
executed continuously for logic requirement.

4. one gcalloc cost comparison:
   my coroutine:                 32B = CoroutineItem (16B) + IEnumerator (16B)
   MonoBehaviour coroutine:     37B
   event combine delegate:      52B

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;

namespace Unicorn
{
    public class CoroutineManager
    {
        static CoroutineManager()
        {
        }

        private CoroutineManager()
        {
            EditorCallback.AttachToUpdate(ExpensiveUpdate);
        }

        /// <summary>
        /// coroutine will be called once immediately after StartCoroutine();
        /// </summary>
        public void StartCoroutine(IEnumerator routine)
        {
            if (null != routine)
            {
                try
                {
                    var needEnqueue = UpdateTools.IsTimeout() || routine.MoveNext();
                    if (needEnqueue)
                    {
                        const bool isRecyclable = true;
                        _pool.Spawn(routine, isRecyclable);
                    }
                }
                catch (Exception ex)
                {
                    Logo.Error("[CoroutineManager.StartCoroutine()] ex={0}", ex);
                }
            }
        }

        /// <summary>
        /// coroutine will be called once immediately after StartCoroutine();
        /// </summary>
        public void StartCoroutine(IEnumerator routine, out CoroutineItem item)
        {
            // StartCoroutine() 不能返回IEnumerator，处理代码会有时序问题.
            item = null;

            if (null != routine)
            {
                try
                {
                    var needEnqueue = UpdateTools.IsTimeout() || routine.MoveNext();
                    if (needEnqueue)
                    {
                        const bool isRecyclable = false;
                        item = _pool.Spawn(routine, isRecyclable);
                    }
                }
                catch (Exception ex)
                {
                    Logo.Error("[CoroutineManager.StartCoroutine()] ex={0}", ex);
                }
            }
        }

        public bool KillCoroutine(IEnumerator routine)
        {
            if (null != routine)
            {
                var count = _pool.Count;
                for (int i = 0; i < count; i++)
                {
                    var item = _pool[i];
                    if (item.routine == routine)
                    {
                        item.Kill();
                        return true;
                    }
                }
            }

            return false;
        }

        public void KillCoroutine(ref IEnumerator routine)
        {
            if (null != routine)
            {
                KillCoroutine(routine);
                routine = null;
            }
        }

        internal void ExpensiveUpdate()
        {
            if (_pool.Count > 0)
            {
                // 1. _currents.Count may increase at item.coroutine.MoveNext() because StartCoroutine() may be called;
                // 2. So we use snapshotCount to make sure we only call the snapshoted items;
                // 3. StopCoroutine() just set item.isDone= true, and never decrease _currents.Count;
                // 4. The Prediate<T> in List.RemoveAll(Predicate<T>) will be called several times, so it can not have any logic;

                var someIsDone = false;
                var snapshotCount = _pool.Count;

                for (var index = 0; index < snapshotCount; ++index)
                {
                    if (UpdateTools.IsTimeout())
                    {
                        break;
                    }

                    var item = _pool[index];
                    if (!item.isDone && !item.isKilled)
                    {
                        try
                        {
                            var routine = item.routine;
                            var isDone = !routine.MoveNext();
                            if (isDone)
                            {
                                item.AddFlag(CoroutineItem.Flag.Done);
                            }
                        }
                        catch (Exception ex)
                        {
                            item.AddFlag(CoroutineItem.Flag.Done);
                            Logo.Error($"[CoroutineManager.Update()] ex={ex}, StackTrace={ex.StackTrace}");
                        }
                    }

                    if (item.isDone || item.isKilled)
                    {
                        someIsDone = true;
                    }
                }

                if (someIsDone)
                {
                    _pool.Recycle();
                }
            }
        }

        internal void Clear()
        {
            _pool.Clear();
        }

        public static readonly CoroutineManager It = new();
        private readonly CoroutinePool _pool = new CoroutinePool();
    }
}