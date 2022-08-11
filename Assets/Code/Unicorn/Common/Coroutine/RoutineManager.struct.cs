//
///********************************************************************
//created:    2022-08-11
//author:     lixianmin
//
//1. Coroutine using is strictly limited in our game.
//
//2. RoutineManger supports "yield return null" usage, and IIsYieldable implemented classes, while "yield return www" is not supported.
//
//3. We can not use TickAmortize to amorize coroutine time, because several coroutines may need to be
//excuted continuously for logic requirement.
//
//4. one gcalloc cost comparison:
//   my coroutine:                16B
//   MonoBehaviour coroutine:     37B
//   event combine delegate:      52B
//
//Copyright (C) - All Rights Reserved
//*********************************************************************/
//using UnityEngine;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//
//namespace Unicorn
//{
//    public static class RoutineManager
//    {
//        private struct CoroutineItem
//        {
//			public CoroutineItem (IEnumerator routine, float endTime)
//            {
//                this.routine = routine;
//                this.isDone  = false;
//				this.endTime = endTime;
//            }
//
//			public override string ToString ()
//			{
//				return string.Format ("[CoroutineItem] routine={0}, isDone={1}"
//				                      , routine, isDone.ToString());
//			}
//
//            public IEnumerator  routine;
//            public bool         isDone;
//			public float		endTime;
//        }
//
//		static RoutineManager ()
//		{
//			EditorCallback.AttachToUpdate(Tick);
//		}
//
//        /// <summary>
//        /// coroutine will be called once immediately after StartCoroutine();
//        /// </summary>
//		public static IEnumerator StartCoroutine (IEnumerator routine)
//        {
//			if (null == routine)
//			{
//				return null;
//			}
//
//            try 
//            {
//                if (routine.MoveNext())
//                {
//                    var item = new CoroutineItem(routine, float.MaxValue);
//                    _currents.Add(item);
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine("[CoroutineManager.StartCoroutine()] ex={0}", ex.ToStringEx());
//            }
//
//			return routine;
//        }
//
//		public static IEnumerator StartCoroutineTimeout (IEnumerator routine, float timeout= _kDefaultTimeout)
//		{
//			if (null == routine)
//			{
//				return null;
//			}
//
//			try 
//			{
//				if (routine.MoveNext())
//				{
//					if (timeout <= 0)
//					{
//						timeout = _kDefaultTimeout;
//					}
//
//					var endTime = Time.time + timeout;
//					var item = new CoroutineItem(routine, endTime);
//					_currents.Add(item);
//				}
//			}
//			catch (Exception ex)
//			{
//				Console.Error.WriteLine("[CoroutineManager.StartCoroutineTimeout()] ex={0}, StackTrace={1}", ex, ex.StackTrace);
//			}
//
//			return routine;
//		}
//
//        /// <summary>
//        /// Stops the coroutine. This method only set item.isDone= true, and the item will be really removed in Tick() method.
//        /// </summary>
//        public static void StopCoroutine (IEnumerator routine)
//        {
//			if (null == routine)
//			{
//				return;
//			}
//
//            var count = _currents.Count;
//
//            for (int index = 0; index < count; ++index)
//            {
//                if (_currents[index].routine == routine)
//                {
//                    var item = _currents[index];
//                    item.isDone = true;
//                    _currents[index] = item;
//                }
//            }
//        }
//
//		public static void StopCoroutine (ref IEnumerator routine)
//		{
//			if (null != routine)
//			{
//				StopCoroutine(routine);
//				routine = null;
//			}
//		}
//        
//        internal static void Tick ()
//        {
//            if (_currents.Count > 0)
//            {
//                // 1. _currents.Count may increase at item.coroutine.MoveNext() because StartCoroutine() may be called;
//                // 2. So we use snapshotCount to make sure we only call the snapshoted items;
//                // 3. StopCoroutine() just set item.isDone= true, and never decrease _currents.Count;
//                // 4. The Prediate<T> in List.RemoveAll(Predicate<T>) will be called several times, so it can not have any logic;
//
//                var someIsDone = false;
//                var snapshotCount = _currents.Count;
//
//                for (int index= 0; index < snapshotCount; ++index)
//                {
//                    var item = _currents[index];
//                    if (!item.isDone)
//                    {
//                        try 
//                        {
//							var routine   = item.routine;
//							var checkDone = routine.Current as IIsYieldable;
//							if (null == checkDone || checkDone.isYieldable)
//							{
//								// item.isDone = (null != terminator && terminator.IsDisposed()) || !routine.MoveNext();
//								item.isDone = !routine.MoveNext();
//							}
//
//							if (Time.time > item.endTime)
//							{
//								throw new TimeoutException(item.ToString());
//							}
//                        }
//                        catch (Exception ex)
//                        {
//                            item.isDone = true;
//                            Console.Error.WriteLine("[CoroutineManager.Tick()] ex={0}, StackTrace={1}", ex, ex.StackTrace);
//                        }
//
//                        if (item.isDone)
//                        {
//                            // item's type is struct, so we need to write it back after modification.
//                            _currents[index] = item;
//                            someIsDone = true;
//                        }
//                    }
//                    else 
//                    {
//                        someIsDone = true;
//                    }
//                }
//
//                if (someIsDone)
//                {
//                    _currents.RemoveAll(item=> item.isDone);
//                }
//            }
//        }
//
//        internal static void Clear ()
//        {
//			if (_currents.Count > 0)
//			{
//				_currents.Clear();
//			}
//        }
//
//		private const float _kDefaultTimeout = 60.0f;
//        private static readonly List<CoroutineItem> _currents = new List<CoroutineItem>(8);
//    }
//}