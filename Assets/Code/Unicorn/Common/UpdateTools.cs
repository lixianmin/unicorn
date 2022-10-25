
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
		internal static void Update()
		{
			_frameStartTime = Time.realtimeSinceStartup;
			var deltaTime = Time.deltaTime;

			var count = _updateItems.Count;
			for (var i = 0; i < count; ++i)
			{
				var updatable = _updateItems[i] as IExpensiveUpdate;
				updatable?.ExpensiveUpdate(deltaTime);
			}
		}

		public static bool IsTimeout(float timeout = 0.1f)
		{
			return Time.realtimeSinceStartup > _frameStartTime + timeout;
		}

		internal static void AttachUpdate(IExpensiveUpdate expensiveUpdate)
		{
			if (null != expensiveUpdate)
			{
				_updateItems.Add(expensiveUpdate);
			}
		}

		internal static void DetachUpdate(IExpensiveUpdate expensiveUpdate)
		{
			if (null != expensiveUpdate)
			{
				_updateItems.Remove(expensiveUpdate);
			}
		}

		// to ensure IsTimeout() is false when Update() is not called (in editor).
		private static float _frameStartTime = float.MaxValue;
		private static readonly  ArrayList _updateItems = new();
	}
}