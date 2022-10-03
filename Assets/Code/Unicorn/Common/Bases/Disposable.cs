
/********************************************************************
created:    2014-06-25
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;

namespace Unicorn
{
	public abstract class Disposable : IDisposable, IIsDisposed
	{
		[Flags]
		private enum Flags : ushort
		{
			None = 0,
			IsDisposed = 1,
			IsManualDisposing = 2,
			CanManualDisposable = 4,
		}

		~Disposable()
		{
			_flags &= ~Flags.IsManualDisposing;
			DisposableRecycler.Recycle(this);
		}

		public void Dispose()
		{
			if (IsDisposed())
			{
				return;
			}

			var isManualDisposing = (_flags & Flags.IsManualDisposing) != 0;
			var canManualDisposable = (_flags & Flags.CanManualDisposable) != 0;
			if (isManualDisposing && !canManualDisposable)
			{
				return;
			}

			try
			{
				_DoDispose(isManualDisposing);
			}
			finally
			{
				GC.SuppressFinalize(this);
				_flags |= Flags.IsDisposed;
			}
		}

		public bool IsDisposed()
		{
			var isDisposed = (_flags & Flags.IsDisposed) != 0;
			return isDisposed;
		}

		internal void EnableManualDisposable()
		{
			_flags |= Flags.CanManualDisposable;
		}

		public void DisableManualDisposable()
		{
			_flags &= ~Flags.CanManualDisposable;
		}

		protected abstract void _DoDispose(bool isManualDisposing);

		private Flags _flags = Flags.IsManualDisposing | Flags.CanManualDisposable;
	}
}