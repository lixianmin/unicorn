
/********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using UnityEngine;

namespace Unicorn
{
	internal static class EditorCallback
	{
		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		public static void AttachToUpdate(Action action)
		{
			if (!Application.isEditor || null == action)
			{
				return;
			}

			if (null == _lpfnAttachToUpdate)
			{
				var type = TypeTools.SearchType("Unique.EditorTools");
				if (null != type)
				{
					TypeTools.CreateDelegate(type, "_AttachToUpdate", out _lpfnAttachToUpdate);
				}
			}

			if (null != _lpfnAttachToUpdate)
			{
				_lpfnAttachToUpdate(action);
			}
		}

		private static Action<Action> _lpfnAttachToUpdate;
	}
}