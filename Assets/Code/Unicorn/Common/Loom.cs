
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

		private static void _RunAsyncAction(object state)
		{
			var action = state as Action;
			CallbackTools.Handle(ref action, "[Loom._RunAsyncAction()]");
		}

		public static void QueueOnMainThread(Action action)
		{
			if (null != action)
			{
				lock (_locker)
				{
					_receivedActions.Add(action);
				}
			}
		}

		internal static void Tick()
		{
			if (_receivedActions.Count > 0)
			{
				var count = _receivedActions.MoveToEx(_tempActions, _locker);
				for (int i = 0; i < count; ++i)
				{
					var action = _tempActions[i] as Action;
					CallbackTools.Handle(ref action, "[Loom._Tick()]");
				}

				_tempActions.Clear();
			}
		}

		private static readonly WaitCallback _lpfnRunAsyncAction = _RunAsyncAction;

		private static readonly object _locker = new();
		private static readonly ArrayList _receivedActions = new();
		private static readonly ArrayList _tempActions = new();
	}
}