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

        public void Add(Type type, IPart part, bool checkDuplicated)
        {
            if (_parts is PartItem[] array)
            {
                if (checkDuplicated)
                {
                    for (var i = 0; i < _size; i++)
                    {
                        var partItem = array[i];
                        // 这里改为按type检查, 因为后面hashtable是按type存储的, 所以array这一步也应该如此检查
                        if (partItem.type == type)
                        {
                            var message = "duplicated partType=" + type.FullName;
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
                    var hashtable = (_cacheHashParts.Count <= 0)
                        ? new Hashtable(_kArraySize)
                        : _cacheHashParts.PopBack() as Hashtable;
                    for (var j = 0; j < _size; j++)
                    {
                        var item = array[j];
                        array[j] = default;
                        hashtable!.Add(item.type, item.part);
                    }

                    _cacheArrayParts.PushBack(array);
                    _parts = hashtable;
                    hashtable!.Add(type, part);
                    _size++;
                }
            }
            else
            {
                var hashtable = _parts as Hashtable;
                hashtable?.Add(type, part);
                _size++;
            }
        }

        public IPart GetPart(Type type)
        {
            if (_parts is PartItem[] array)
            {
                for (var i = 0; i < _size; i++)
                {
                    var partItem = array[i];
                    if (partItem.type == type)
                    {
                        return partItem.part;
                    }
                }

                return null;
            }

            var hashtable = _parts as Hashtable;
            return hashtable?[type] as IPart;
        }

        public void GetParts(List<IPart> results)
        {
            if (results != null)
            {
                if (_parts is PartItem[] array)
                {
                    for (var i = 0; i < _size; i++)
                    {
                        var partItem = array[i];
                        var part = partItem.part;
                        results.Add(part);
                    }
                }
                else
                {
                    var hashtable = _parts as Hashtable;
                    var iter = hashtable!.GetEnumerator();
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
            if (_parts is PartItem[] array)
            {
                for (var i = 0; i < _size; i++)
                {
                    var partItem = array[i];
                    if (partItem.type == type)
                    {
                        var part = partItem.part;
                        if (part is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }

                        _size--;
                        array[i] = array[_size];
                        array[_size] = default;
                        return true;
                    }
                }

                return false;
            }

            var hashtable = _parts as Hashtable;
            if (hashtable![type] is IDisposable disposable2)
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
            if (_parts is PartItem[] array)
            {
                for (var i = 0; i < _size; i++)
                {
                    var partItem = array[i];
                    if (partItem.part is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }

                    array[i] = default;
                }

                _cacheArrayParts.PushBack(array);
            }
            else
            {
                var hashtable = _parts as Hashtable;
                hashtable.DisposeAllAndClear();
                _cacheHashParts.PushBack(hashtable);
            }

            _parts = null;
            _size = 0;
        }

        private const int _kArraySize = 16;

        private int _size;

        private object _parts = _cacheArrayParts.Count <= 0 ? new PartItem[_kArraySize] : _cacheArrayParts.PopBack() as PartItem[];

        private static readonly Deque _cacheArrayParts = new();

        private static readonly Deque _cacheHashParts = new();
    }
}