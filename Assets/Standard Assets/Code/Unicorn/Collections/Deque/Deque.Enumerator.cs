
/*********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections;

namespace Unicorn.Collections
{
	partial class Deque<T>
	{
		public struct Enumerator
		{
            public Enumerator (Deque<T> queue)
			{
				_queue  	= queue;
				_version	= queue._version;
				_current 	= -1;
			}
			
			public bool MoveNext ()
			{
				if (_version != _queue._version)
				{
					throw new InvalidOperationException("Invalid queue version");
				}

				if (_current >= _queue._size - 1)
				{
					_current = -1;
					return false;
				}

				_current++;
				return true;
			}
			
			public void Reset ()
			{
				if (_version != _queue._version)
				{
					throw new InvalidOperationException ();
				}

				_current = -1;
			}
			
			public T Current
			{
				get 
				{
					if (_version != _queue._version || _current < 0 || _current >= _queue._size)
					{
						throw new InvalidOperationException ();
					}

					return _queue._items [(_queue._head + _current) % _queue._capacity];
				}
			}
			
            private readonly Deque<T> _queue;
			private int		_current;
			private int 	_version;
		}
	}
}