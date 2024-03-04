
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
	partial class SortedTable<TKey, TValue>
	{
        [Serializable]
		public struct Enumerator : IDictionaryEnumerator, IEnumerator
		{
            internal Enumerator (SortedTable<TKey, TValue> table)
			{
				_table = table;
				_index = 0;

				_pair  = default;
                _version = _table._version;
			}
			
			public bool MoveNext ()
			{
				if (null == _table)
				{
					return false;
				}

                if (_version != _table._version)
                {
                    throw new InvalidOperationException("Invalid table version");
                }

				var count = _table._size;
				if (_index < count)
				{
					var key = _table._keys[_index];
					var val = _table._values[_index];
					_pair = new KeyValuePair<TKey, TValue>(key, val);
					++_index;

					return true;
				}

				_index = count + 1;
				_pair  = default;
				return false;
			}

			public void Reset ()
			{
				if (null == _table)
				{
					return;
				}

                if (_version != _table._version)
                {
                    throw new InvalidOperationException("Invalid table version");
                }

				_index = 0;
				_pair  = default;
			}

            public KeyValuePair<TKey, TValue> Current => _pair;

            object IEnumerator.Current => (this as IDictionaryEnumerator).Entry;

			DictionaryEntry IDictionaryEnumerator.Entry => new(_pair.Key, _pair.Value);

			object IDictionaryEnumerator.Key => _pair.Key;

			object IDictionaryEnumerator.Value => _pair.Value;

			private readonly SortedTable<TKey, TValue> _table;
			private int _index;

			private KeyValuePair<TKey, TValue> _pair;
            private int _version;
		}
	}
}