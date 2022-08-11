
/********************************************************************
created:    2013-12-23
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections;

namespace Unicorn
{
	internal static class DisposableRecycler
	{
		internal static void Recycle(IDisposable obj)
		{
			if (null != obj)
			{
				lock (_locker)
				{
					_receivedItems.Add(obj);
				}
			}
		}

		internal static void Tick()
		{
			if (_receivedItems.Count > 0)
			{
				var count = _receivedItems.MoveToEx(_tempItems, _locker);
				for (int i = 0; i < count; ++i)
				{
					var obj = _tempItems[i] as IDisposable;
					obj.Dispose();
				}

				_tempItems.Clear();
			}
		}

		private static readonly object _locker = new object();
		private static readonly ArrayList _receivedItems = new ArrayList();
		private static readonly ArrayList _tempItems = new ArrayList();
	}
}