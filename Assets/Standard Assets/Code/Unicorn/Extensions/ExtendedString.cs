/*********************************************************************
created:	2014-04-25
author:		lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Unicorn
{
    public static class ExtendedString
    {
        public static TEnum ToEnumEx<TEnum>(this string name) where TEnum : struct
        {
            Enum.TryParse<TEnum>(name, out var result);
            return result;
        }

        public static bool IsNullOrEmptyEx(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        public static bool StartsWithEx(this string text, string candidate, CompareOptions options)
        {
            if (null != text && null != candidate)
            {
                return _currentCompareInfo.IsPrefix(text, candidate, options);
            }

            return false;
        }

        public static bool StartsWithEx(this string text, string[] candidates, CompareOptions options)
        {
            if (null != text && null != candidates && candidates.Length > 0)
            {
                foreach (var item in candidates)
                {
                    if (text.StartsWithEx(item, options))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool EndsWithEx(this string text, string candidate, CompareOptions options)
        {
            if (null != text && null != candidate)
            {
                return _currentCompareInfo.IsSuffix(text, candidate, options);
            }

            return false;
        }

        public static bool EndsWithEx(this string text, string[] candidates, CompareOptions options)
        {
            if (null != text && null != candidates && candidates.Length > 0)
            {
                foreach (var item in candidates)
                {
                    if (text.EndsWithEx(item, options))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static string JoinEx(this string separator, IEnumerable collection)
        {
            if (null == separator || null == collection)
            {
                return string.Empty;
            }

            StringBuilder sbText = null;
            var iter = collection.GetEnumerator();

            while (iter.MoveNext())
            {
                var item = iter.Current;
                if (null != item)
                {
                    sbText = new StringBuilder(128);
                    sbText.Append(item);
                    break;
                }
            }

            while (iter.MoveNext())
            {
                var item = iter.Current;
                if (null != item)
                {
                    sbText.Append(separator);
                    sbText.Append(item);
                }
            }

            return null != sbText ? sbText.ToString() : string.Empty;
        }

        public static string JoinEx<T>(this string separator
            , IEnumerable<T> collection
            , Func<T, string> processor)
        {
            if (null == separator || null == collection)
            {
                return string.Empty;
            }

            StringBuilder sbText = null;
            var iter = collection.GetEnumerator();

            while (iter.MoveNext())
            {
                var item = iter.Current;
                var result = processor(item);
                if (null != result)
                {
                    sbText = new StringBuilder(128);
                    sbText.Append(result);
                    break;
                }
            }

            while (iter.MoveNext())
            {
                var item = iter.Current;
                var result = processor(item);
                if (null != result)
                {
                    sbText.Append(separator);
                    sbText.Append(result);
                }
            }

            return null != sbText ? sbText.ToString() : string.Empty;
        }

        public static int ReversedCompareToEx(this string lhs, string rhs, int deltaIndex = 0)
        {
            if (object.ReferenceEquals(lhs, rhs))
            {
                return 0;
            }
            else if (null == lhs)
            {
                return -1;
            }
            else if (null == rhs)
            {
                return 1;
            }

            var count = lhs.Length;
            if (count < rhs.Length)
            {
                return -1;
            }
            else if (count > rhs.Length)
            {
                return 1;
            }

            for (int i = count - deltaIndex - 1; i >= 0; --i)
            {
                var a = lhs[i];
                var b = rhs[i];
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

        /// <summary>
        /// 计算字符串编辑距离 https://en.wikipedia.org/wiki/Levenshtein_distance
        /// </summary>
        /// <returns>The edit distance ex.</returns>
        /// <param name="s">S.</param>
        /// <param name="t">T.</param>
        public static int GetEditDistanceEx(this string s, string t)
        {
            // degenerate cases
            if (s == t)
            {
                return 0;
            }

            var sLength = null != s ? s.Length : 0;
            var tLength = null != t ? t.Length : 0;

            if (sLength == 0)
            {
                return tLength;
            }

            if (tLength == 0)
            {
                return sLength;
            }

            // create two work vectors of integer distances
            int[] v0 = new int[tLength + 1];
            int[] v1 = new int[tLength + 1];

            // initialize v0 (the previous row of distances)
            // this row is A[0][i]: edit distance for an empty s
            // the distance is just the number of characters to delete from t
            var v0Length = v0.Length;
            for (int i = 0; i < v0Length; i++)
            {
                v0[i] = i;
            }

            for (int i = 0; i < sLength; i++)
            {
                // calculate v1 (current row distances) from the previous row v0

                // first element of v1 is A[i+1][0]
                //   edit distance is delete (i+1) chars from s to match empty t
                v1[0] = i + 1;

                // use formula to fill in the rest of the row
                for (int j = 0; j < tLength; j++)
                {
                    var cost = (s[i] == t[j]) ? 0 : 1;
                    v1[j + 1] = _Minimum(v1[j] + 1, v0[j + 1] + 1, v0[j] + cost);
                }

                // copy v1 (current row) to v0 (previous row) for next iteration
                for (int j = 0; j < v0Length; j++)
                {
                    v0[j] = v1[j];
                }
            }

            return v1[tLength];
        }

        private static int _Minimum(int a, int b, int c)
        {
            if (a < b)
            {
                if (a < c)
                {
                    return a;
                }
                else
                {
                    return c;
                }
            }
            else if (b < c)
            {
                return b;
            }
            else
            {
                return c;
            }
        }

        private static readonly CompareInfo _currentCompareInfo = CultureInfo.CurrentCulture.CompareInfo;
    }
}