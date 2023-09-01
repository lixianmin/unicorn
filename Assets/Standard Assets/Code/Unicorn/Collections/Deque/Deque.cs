
/*********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace Unicorn.Collections
{
    public partial class Deque<T> : IEnumerable<T>
	{
		public Deque () : this (0)
		{

		}

		public Deque (int capacity)
		{
            _capacity = _CalcInitialCapacity(capacity);
            _items = new T[_capacity];
		}

        private int _CalcInitialCapacity (int candidate) 
        {
            int capacity = 2;

            while (capacity < candidate)
            {
                capacity <<= 1;
            }

            return capacity;
        }

		public void PushBack (T item)
		{
			++_version;

			if (_size == _capacity)
			{
                _capacity <<= 1;
				var items = new T[_capacity];
				CopyTo(items, 0);

				_items = items;
				_head  = 0;
				_tail  = _size;
			}

			_items[_tail] = item;
            _tail = (_tail + 1) & (_capacity - 1);
			_size++;
		}

		public T PopFront ()
		{
			++_version;

			if (_size < 1)
			{
				throw new InvalidOperationException ();
			}

			var result = _items [_head];
            _items[_head] = default(T);
            _head = (_head + 1) & (_capacity - 1);
			_size--;

			return result;
		}

		public T PopBack ()
		{
			++_version;
			
			if (_size < 1)
			{
				throw new InvalidOperationException ();
			}

            _tail = (_tail - 1) & (_capacity - 1);
			var result = _items[_tail];
            _items[_tail] = default(T);
			--_size;
			
			return result;
		}

		public T Front ()
		{
			if (_size < 1)
			{
				throw new InvalidOperationException ();
			}

			var result = _items[_head];
			return result;
		}

		public T Back ()
		{
			if (_size < 1)
			{
				throw new InvalidOperationException ();
			}

            var index = (_tail - 1) & (_capacity - 1);
			var result = _items[index];
			return result;
		}

		public void CopyTo (Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException ("array");
			}

			if (index < 0)
			{
				throw new ArgumentOutOfRangeException ("index");
			}

			if (array.Rank > 1 || (index != 0 && index >= array.Length) || this._size > array.Length - index)
			{
				throw new ArgumentException ();
			}

			int tailCount  = _items.Length - _head;
			Array.Copy (_items, _head, array, index, Math.Min (_size, tailCount));

			if (_size > tailCount)
			{
				Array.Copy (_items, 0, array, index + tailCount, _size - tailCount);
			}
		}

		public T this [int index]
		{
			get
			{
				if (index >= 0 && index < _size)
				{
                    var realIndex = (_head + index) & (_capacity - 1);
					var result = _items[realIndex];
					return result;
				}

                return default(T);
			}
		}

		public bool Contains (T item)
		{
			return IndexOf(item) >= 0;
		}

		public int IndexOf(T item)
		{
			if (null != _items && _size > 0)
            {
				var comparer = EqualityComparer<T>.Default;
				for (int i = 0; i < _size; ++i)
				{
					var index = (_head + i) & (_capacity - 1);
					if (comparer.Equals(_items[index], item))
					{
						return i;
					}
				}
			}

			return -1;
		}

		public void RemoveAt (int index)
		{
			if (index < 0 || index >= _size)
			{
				throw new ArgumentOutOfRangeException("index is out of range.");
			}
			
			int beginIndex = _head + index;
			int endIndex = _head + _size;

            int p = beginIndex & (_capacity - 1);
			int q = 0;

			for (int i= beginIndex; i < endIndex; ++i)
			{
				q = p;
                p = (i + 1) & (_capacity - 1);
				_items[q] = _items[p];
			}

			++_version;
			_size--;
			_tail = q;
            _items[q] = default;
		}
		
		public void Clear ()
		{
			++_version;

			if (_size > 0)
			{
				Array.Clear(_items, 0, _capacity);
			}

			_head = 0;
			_size = 0;
			_tail = 0;
		}

		public Enumerator GetEnumerator ()
		{
			return new Enumerator(this);
		}
		
        IEnumerator<T> IEnumerable<T>.GetEnumerator ()
        {
            return _GetEnumerator();
        }

		IEnumerator IEnumerable.GetEnumerator()
		{
            return _GetEnumerator();
		}

        private IEnumerator<T> _GetEnumerator ()
        {
            var lastVersion = _version;

            for (int i= 0; i< _size; ++i)
            {
                var index = (_head + i) & (_capacity - 1);
                var result = _items[index];
                yield return result;

                if (lastVersion != _version)
                {
                    throw new InvalidOperationException("Invalid Deque version");
                }
            }
        }

		public override string ToString ()
		{
			var sb = new System.Text.StringBuilder(32);
			sb.AppendFormat("Count={0}, _capacity={1}, _head={2}, _tail={3}"
			                , Count, _capacity, _head, _tail);
			
			sb.Append("\n     items=[");
			sb.Append(", ".Join(this));
			sb.Append("]");
			
			var text = sb.ToString();
			return text;
		}

		public int Count { get { return _size; } }

		private int _head;
		private int _tail;	
		private int _size;
		private int _capacity;
		private int _version;
		private T[] _items;

//        private static readonly bool _isNullable = !typeof(T).IsSubclassOf(typeof(ValueType));
	}
}