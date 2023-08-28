
/********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections;

namespace Unicorn
{
    internal class CoroutinePool
    {
		internal CoroutinePool ()
		{
			_capacity = 32;
			_items = new CoroutineItem[_capacity];
		}

		internal CoroutineItem Spawn (IEnumerator routine, bool isRecyclable)
		{
			AssertTools.IsNotNull (routine);

			CoroutineItem item;
			var checkIndex = _size;
			if (checkIndex < _capacity && null != _items[checkIndex])
			{
				item = _items[checkIndex];
				item.isDone = false;
				item.isKilled = false;
			}
			else
			{
				if (checkIndex + 1 > _capacity)
				{
					_capacity = ArrayTools.EnsureCapacity(_capacity, checkIndex + 1);
					Array.Resize(ref _items, _capacity);
				}

				item = new CoroutineItem();
				_items [checkIndex] = item;
			}

			++_size;
			item.routine = routine;
			item.isRecyclable = isRecyclable;

			return item;
		}

		internal void Recycle ()
		{
			var count = _size;
			
			int i;
			for (i = 0; i < count; i++)
			{
				var item = _items[i];
				if (item.isDone || item.isKilled)
				{
					item.routine = null;
					break;
				}
			}
			
			if (i < count)
			{
				for (int j = i + 1; j < count; j++)
				{
					var item = _items[j];
					if (item.isDone || item.isKilled)
					{
						if (item.isRecyclable)
						{
							item.routine = null;
						}
						else
						{
							_items[j] = null;
						}
					}
					else
					{
						(_items[i], _items[j]) = (_items[j], _items[i]);
						++i;
						// os.swap(ref _items[i++], ref _items[j]);
					}
				}
				
				_size = i;
			}
		}

		internal CoroutineItem this[int index]
		{
			get => _items[index];

			set
			{
				_items [index] = value;
			}
		}

		internal void Clear ()
		{
			_size = 0;
		}

		internal int Count => _size;

		private int _size;
		private int _capacity;
		private CoroutineItem[] _items;
    }
}