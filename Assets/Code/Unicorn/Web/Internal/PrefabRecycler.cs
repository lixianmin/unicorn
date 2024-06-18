/********************************************************************
created:    2022-08-13
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.Collections.Generic;
using System.Text;

namespace Unicorn.Web.Internal
{
    internal class RecyclerItem
    {
        public WebPrefab prefab;
        public int counter;
    }

    public static class PrefabRecycler
    {
        public static void TryAddPrefab(string localPath, WebPrefab prefab)
        {
            if (prefab != null && !_cache.TryGetValue(localPath, out var cached))
            {
                cached = new RecyclerItem
                {
                    prefab = prefab
                };
                _cache.Add(localPath, cached);
            }
        }

        public static void AddReference(string localPath)
        {
            if (_cache.TryGetValue(localPath, out var cached))
            {
                cached.counter++;
            }
        }

        public static void RemoveReference(string localPath)
        {
            if (_cache.TryGetValue(localPath, out var cached))
            {
                cached.counter--;
                if (cached.counter == 0)
                {
                    _cache.Remove(localPath);
                }
            }
        }

        public static void PrintSummary()
        {
            var sb = new StringBuilder();
            sb.Append($"count={_cache.Count}\n");
            foreach (var item in _cache)
            {
                sb.Append(item.Key);
                sb.Append(": ");
                sb.Append(item.Value.counter);
                sb.Append("\n");
            }

            Logo.Warn(sb.ToString());
        }

        private static readonly Dictionary<string, RecyclerItem> _cache = new(128);
    }
}