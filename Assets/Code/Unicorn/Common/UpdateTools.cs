
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

			var count = _updateItems.Count;
			for (var i = 0; i < count; ++i)
			{
				var updatable = _updateItems[i] as IUpdatable;
				updatable?.Update();
			}
		}

		public static bool IsTimeout(float timeout = 0.1f)
		{
			return Time.realtimeSinceStartup > _frameStartTime + timeout;
		}

		internal static void AttachUpdate(IUpdatable updatable)
		{
			if (null != updatable)
			{
				_updateItems.Add(updatable);
			}
		}

		internal static void DetachUpdate(IUpdatable updatable)
		{
			if (null != updatable)
			{
				_updateItems.Remove(updatable);
			}
		}

		// to ensure IsTimeout() is false when Update() is not called (in editor).
		private static float _frameStartTime = float.MaxValue;
		private static readonly  ArrayList _updateItems = new ArrayList();
	}
}