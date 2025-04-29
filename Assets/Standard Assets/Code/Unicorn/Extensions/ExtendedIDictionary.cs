/********************************************************************
created:    2020-10-20
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Unicorn.Collections;

namespace Unicorn
{
    public static class ExtendedIDictionary
    {
        public static bool IsNullOrEmpty<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return dict == null || dict.Count == 0;
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            return dict.Get(key, default);
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            if (null != dict && dict.TryGetValue(key, out var v))
            {
                return v;
            }

            return defaultValue;
        }

        public static TValue SetDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            return dict.SetDefault(key, default);
        }

        public static TValue SetDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
            TValue defaultValue)
        {
            if (null != dict)
            {
                if (!dict.TryGetValue(key, out var value))
                {
                    value = defaultValue;
                    dict.Add(key, value);
                }

                return value;
            }

            return default;
        }

        public static TValue Add<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue v)
        {
            dict?.Add(key, v);
            return v;
        }

        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictTo,
            IDictionary<TKey, TValue> dictFrom) where TKey : IComparable
        {
            if (null != dictTo && null != dictFrom)
            {
                using var e = dictFrom.GetEnumerator();
                while (e.MoveNext())
                {
                    var pair = e.Current;
                    dictTo[pair.Key] = pair.Value;
                }
            }
        }

        // [Obsolete("use DisposeAll().Clear() for instead")]
        // public static void DisposeAllAndClear(this IDictionary table)
        // {
        //     var count = table?.Count;
        //     if (count > 0)
        //     {
        //         var iter = table.GetEnumerator();
        //         while (iter.MoveNext())
        //         {
        //             if (iter.Value is IDisposable item)
        //             {
        //                 item.Dispose();
        //             }
        //         }
        //
        //         table.Clear();
        //     }
        // }

        public static IDictionary DisposeAll(this IDictionary table)
        {
            var count = table?.Count ?? 0;
            if (count > 0)
            {
                var snapshot = SlicePool.Get<IDisposable>();
                var iter = table!.GetEnumerator();
                while (iter.MoveNext())
                {
                    if (iter.Value is IDisposable item)
                    {
                        snapshot.Add(item);
                    }
                }

                // 回收iterator
                if (iter is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                // item.Dispose()方法, 有可能会修改原始table本身
                for (var i = 0; i < snapshot.Size; i++)
                {
                    var item = snapshot[i];
                    item.Dispose();
                }

                SlicePool.Return(snapshot);
            }

            return table;
        }
    }
}