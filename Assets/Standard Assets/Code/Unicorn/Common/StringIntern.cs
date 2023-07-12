
/*********************************************************************
created:    2014-12-02
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections.Generic;

namespace Unicorn
{
    public class StringIntern
    {
        public StringIntern()
        {
            _NormalInit();
        }

        public StringIntern(StringIntern other)
        {
            if (null != other)
            {
                var count = other._texts.Length;
                _texts = new string[count];

                for (int i = 0; i < count; ++i)
                {
                    _texts[i] = other._texts[i];
                }

                _comparer = other._comparer;
            }
            else
            {
                _NormalInit();
            }
        }

        private void _NormalInit()
        {
            _texts = _emptyKeys;
            _comparer = ReversedStringComparer.It;
        }

        public string Intern(string text)
        {
            if (null == text)
            {
                throw new ArgumentNullException("text is null");
            }

            int index = _BinarySearch(text);
            if (index >= 0)
            {
                return _texts[index];
            }

            _Insert(~index, text);
            return text;
        }

        public string IsInterned(string text)
        {
            if (null == text)
            {
                throw new ArgumentNullException("text is null");
            }

            int index = _BinarySearch(text);
            if (index >= 0)
            {
                return _texts[index];
            }

            return null;
        }

        public string Intern(char[] buffer)
        {
            if (null == buffer)
            {
                throw new ArgumentNullException("buffer is null");
            }

            return Intern(buffer, 0, buffer.Length);
        }

        public string Intern(char[] buffer, int startIndex, int count)
        {
            if (null == buffer)
            {
                throw new ArgumentNullException("key is null");
            }

            if (startIndex < 0 || startIndex >= buffer.Length || count < 0 || startIndex + count > buffer.Length)
            {
                throw new ArgumentNullException("Invalid startIndex or count");
            }

            int index = _BinarySearch(buffer, startIndex, count);
            if (index >= 0)
            {
                return _texts[index];
            }

            var text = new string(buffer, startIndex, count);
            _Insert(~index, text);
            return text;
        }

        public string IsInterned(char[] buffer)
        {
            if (null == buffer)
            {
                throw new ArgumentNullException("buffer is null");
            }

            return IsInterned(buffer, 0, buffer.Length);
        }

        public string IsInterned(char[] buffer, int startIndex, int count)
        {
            if (null == buffer)
            {
                throw new ArgumentNullException("key is null");
            }

            if (startIndex < 0 || startIndex >= buffer.Length || count < 0 || startIndex + count > buffer.Length)
            {
                throw new ArgumentNullException("Invalid startIndex or count");
            }

            int index = _BinarySearch(buffer, startIndex, count);
            if (index >= 0)
            {
                return _texts[index];
            }

            return null;
        }

        public void TrimExcess()
        {
            int num = (int)((double)_texts.Length * 0.9);

            if (_size < num)
            {
                Capacity = _size;
            }
        }

        private int _BinarySearch(string key)
        {
            if (_texts.Length > 0)
            {
                int index = Array.BinarySearch(_texts, 0, _size, key, _comparer);
                return index;
            }

            return -1;
        }

        private int _BinarySearch(char[] key, int startIndex, int count)
        {
            int i = -1;
            int j = _size;

            while (i + 1 != j)
            {
                int mid = i + (j - i >> 1);
                if (_ReversedCompare(_texts[mid], key, startIndex, count) < 0)
                {
                    i = mid;
                }
                else
                {
                    j = mid;
                }
            }

            if (j == _size || _ReversedCompare(_texts[j], key, startIndex, count) != 0)
            {
                j = ~j;
            }

            return j;
        }

        private static int _ReversedCompare(string lhs, char[] rhs, int startIndex, int count)
        {
            var compareLength = lhs.Length;

            if (compareLength < count)
            {
                return -1;
            }
            else if (compareLength > count)
            {
                return 1;
            }

            for (int i = compareLength - 1, j = startIndex + count - 1; i >= 0; --i, --j)
            {
                var a = lhs[i];
                var b = rhs[j];
                if (a < b)
                {
                    return -1;
                }
                else if (a > b)
                {
                    return 1;
                }
            }

            return 0;
        }

        private void _Insert(int index, string key)
        {
            if (_size == _texts.Length)
            {
                _EnsureCapacity(_size + 1);
            }

            if (index < _size)
            {
                Array.Copy(_texts, index, _texts, index + 1, _size - index);
            }

            _texts[index] = key;

            ++_size;
        }

        private void _EnsureCapacity(int min)
        {
            var num = (_texts.Length == 0) ? 4 : (_texts.Length * 2);
            const int max = 2146435071;
            if (num > max)
            {
                num = max;
            }
            else if (num < min)
            {
                num = min;
            }

            Capacity = num;
        }

        public int Capacity
        {
            get
            {
                return _texts.Length;
            }

            set
            {
                if (value != _texts.Length)
                {
                    if (value < _size)
                    {
                        throw new ArgumentOutOfRangeException("New capacity is smaller than already.");
                    }

                    if (value > 0)
                    {
                        var destKeys = new string[value];

                        if (_size > 0)
                        {
                            Array.Copy(_texts, 0, destKeys, 0, _size);
                        }

                        _texts = destKeys;
                        return;
                    }

                    _texts = _emptyKeys;
                }
            }
        }

        public int Count { get { return _size; } }

        private string[] _texts;
        private int _size;
        private IComparer<string> _comparer;

        private static readonly string[] _emptyKeys = new string[0];
    }
}