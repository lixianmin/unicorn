
/********************************************************************
created:    2019-08-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using Unicorn.Collections;

namespace Unicorn
{
	internal class EntityTable
	{
		private struct PartItem
		{
			public Type type;
			public IPart part;
		}

		public EntityTable()
		{
			_parts = ((_cacheArrayParts.Count <= 0) ? new PartItem[_kArraySize] : (_cacheArrayParts.PopBack() as PartItem[]));
		}

		public void Add(Type type, IPart part, bool checkDuplicated)
		{
			PartItem[] array = _parts as PartItem[];
			if (array != null)
			{
				if (checkDuplicated)
				{
					for (int i = 0; i < _size; i++)
					{
						PartItem partItem = array[i];
						if (partItem.part == part)
						{
							string message = "duplicated partType=" + type.FullName;
							throw new ArgumentException(message);
						}
					}
				}

				if (_size < _kArraySize)
				{
					array[_size] = new PartItem
					{
						type = type,
						part = part
					};
					_size++;
				}
				else
				{
					var hashtable = (_cacheHashParts.Count <= 0) ? new Hashtable(_kArraySize) : (_cacheHashParts.PopBack() as Hashtable);
					for (int j = 0; j < _size; j++)
					{
						PartItem item = array[j];
						array[j] = default(PartItem);
						hashtable.Add(item.type, item.part);
					}

					_cacheArrayParts.PushBack(array);
					_parts = hashtable;
					hashtable.Add(type, part);
					_size++;
				}
			}
			else
			{
				var hashtable = _parts as Hashtable;
				hashtable.Add(type, part);
				_size++;
			}
		}

		public IPart GetPart(Type type)
		{
			PartItem[] array = _parts as PartItem[];
			if (array != null)
			{
				for (int i = 0; i < _size; i++)
				{
					PartItem partItem = array[i];
					if (partItem.type == type)
					{
						return partItem.part;
					}
				}

				return null;
			}

			var hashtable = _parts as Hashtable;
			return hashtable[type] as IPart;
		}

		public void GetParts(List<IPart> results)
		{
			if (results != null)
			{
				PartItem[] array = _parts as PartItem[];
				if (array != null)
				{
					for (int i = 0; i < _size; i++)
					{
						var partItem = array[i];
						IPart part = partItem.part;
						results.Add(part);
					}
				}
				else
				{
					var hashtable = _parts as Hashtable;
					IDictionaryEnumerator iter = hashtable.GetEnumerator();
					while (iter.MoveNext())
					{
						var part = iter.Value as IPart;
						results.Add(part);
					}
				}
			}
		}

		public bool Remove(Type type)
		{
			PartItem[] array = _parts as PartItem[];
			if (array != null)
			{
				for (int i = 0; i < _size; i++)
				{
					PartItem partItem = array[i];
					if (partItem.type == type)
					{
						var part = partItem.part;
						var disposable = part as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}

						_size--;
						array[i] = array[_size];
						array[_size] = default(PartItem);
						return true;
					}
				}
				return false;
			}

			var hashtable = _parts as Hashtable;
			IDisposable disposable2 = hashtable[type] as IDisposable;
			if (disposable2 != null)
			{
                disposable2.Dispose();
                _size--;
				hashtable.Remove(type);
				return true;
			}

			return false;
		}

		public void Dispose()
		{
			PartItem[] array = _parts as PartItem[];
			if (array != null)
			{
				for (int i = 0; i < _size; i++)
				{
					PartItem partItem = array[i];
					var disposable = partItem.part as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}

					array[i] = default(PartItem);
				}
				_cacheArrayParts.PushBack(array);
			}
			else
			{
				var hashtable = _parts as Hashtable;
				hashtable.DisposeAllAndClearEx();
				_cacheHashParts.PushBack(hashtable);
			}

			_parts = null;
			_size = 0;
		}

        private const int _kArraySize = 16;

		private int _size;

		private object _parts;

		private static readonly Deque _cacheArrayParts = new Deque();

		private static readonly Deque _cacheHashParts = new Deque();
	}
}