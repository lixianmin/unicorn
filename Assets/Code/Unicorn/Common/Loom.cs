
/********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Threading;
using System.Collections;

namespace Unicorn
{
	public static class Loom
	{
		public static void RunAsync(Action action)
		{
			if (null != action)
			{
				ThreadPool.QueueUserWorkItem(_lpfnRunAsyncAction, action);
			}
		}

		public static void RunOnMainThread(Action action)
		{
			if (null != action)
			{
				lock (_locker)
				{
					_receivedActions.Add(action);
				}
			}
		}

		internal static void Update()
		{
			lock (_locker)
			{
				var count = _receivedActions.Count;
				if (count > 0)
				{
					_tempActions.AddRange(_receivedActions);
					_receivedActions.Clear();
				}
			}

			var actionCount = _tempActions.Count;
			if (actionCount > 0)
			{
				for (var i = 0; i < actionCount; i++)
				{
					var action = _tempActions[i] as Action;
					CallbackTools.Handle(ref action , "[Loom.Update()]");
				}
				
				_tempActions.Clear();
			}
		}

		private static readonly WaitCallback _lpfnRunAsyncAction = state =>
		{
			var action = state as Action;
			CallbackTools.Handle(ref action, "[Loom._RunAsyncAction()]");
		};

		private static readonly object _locker = new();
		private static readonly ArrayList _receivedActions = new();
		private static readonly ArrayList _tempActions = new();
	}
}