
/********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Threading;
using System.Collections.Generic;
using Unicorn.Collections;
using UnityEngine;

namespace Unicorn
{
	public static class Loom
	{
		private struct Item
		{
			public Action aciton;
			public float runTime;
		}
		
		public static void RunAsync(Action action)
		{
			if (null != action)
			{
				ThreadPool.QueueUserWorkItem(_lpfnRunAsyncAction, action);
			}
		}

		public static void RunDelayed(Action action, float delayedSeconds = 0)
		{
			if (null != action)
			{
				lock (_locker)
				{
					_receivedActions.Add(new Item{aciton = action, runTime = _currentTime + delayedSeconds});
				}
			}
		}

		internal static void Update()
		{
			lock (_locker)
			{
				_currentTime = Time.time;
				var count = _receivedActions.Count;
				if (count > 0)
				{
					for (int i = 0; i < count; i++)
					{
						var item = _receivedActions[i];
						_delayedActions.Enqueue(item.aciton, item.runTime);
					}

					_receivedActions.Clear();
				}
			}
			
			while (_delayedActions.TryPeek(out var action, out var runTime) && _currentTime >= runTime && !UpdateTools.IsTimeout())
			{
				CallbackTools.Handle(ref action , "[Loom.Update()]");
				_delayedActions.Dequeue();
			}
		}

		private static readonly WaitCallback _lpfnRunAsyncAction = state =>
		{
			var action = state as Action;
			CallbackTools.Handle(ref action, "[Loom._RunAsyncAction()]");
		};

		private static readonly object _locker = new();
		private static readonly List<Item> _receivedActions = new();
		private static readonly PriorityQueue<Action, float> _delayedActions = new();
		private static float _currentTime;
	}
}