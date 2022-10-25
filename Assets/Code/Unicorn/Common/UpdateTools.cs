
/********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using UnityEngine;
using System.Collections;

namespace Unicorn
{
	public static class UpdateTools
	{
		internal static void ExpensiveUpdate(float deltaTime)
		{
			_frameStartTime = Time.realtimeSinceStartup;

			var count = _updateItems.Count;
			for (var i = 0; i < count; ++i)
			{
				var updatable = _updateItems[i] as IExpensiveUpdater;
				updatable?.ExpensiveUpdate(deltaTime);
			}
		}

		public static bool IsTimeout(float timeout = 0.1f)
		{
			return Time.realtimeSinceStartup > _frameStartTime + timeout;
		}

		internal static void AttachUpdate(IExpensiveUpdater expensiveUpdater)
		{
			if (null != expensiveUpdater)
			{
				_updateItems.Add(expensiveUpdater);
			}
		}

		internal static void DetachUpdate(IExpensiveUpdater expensiveUpdater)
		{
			if (null != expensiveUpdater)
			{
				_updateItems.Remove(expensiveUpdater);
			}
		}

		// to ensure IsTimeout() is false when Update() is not called (in editor).
		private static float _frameStartTime = float.MaxValue;
		private static readonly  ArrayList _updateItems = new();
	}
}