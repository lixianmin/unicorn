
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

		internal static void Update()
		{
			lock (_locker)
			{
				if (_receivedItems.Count > 0)
				{
					_tempItems.AddRange(_receivedItems);
					_receivedItems.Clear();
				}
			}

			{
				var count = _tempItems.Count;
				if (count > 0)
				{
					for (var i = 0; i < count; i++)
					{
						var obj = _tempItems[i] as IDisposable;
						obj.Dispose();
					}
					
					_tempItems.Clear();
				}
			}
		}

		private static readonly object _locker = new object();
		private static readonly ArrayList _receivedItems = new ArrayList();
		private static readonly ArrayList _tempItems = new ArrayList();
	}
}