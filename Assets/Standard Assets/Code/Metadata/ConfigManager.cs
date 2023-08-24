/********************************************************************
created:    2014-01-13
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Unicorn;
using Unicorn.Collections;

namespace Metadata
{
    public class ConfigManager : IDisposable
    {
        public void Dispose()
        {
            Clear();
        }

        public bool AddConfig(Config config)
        {
            if (null == config)
            {
                return false;
            }

            var type = config.GetType();
            var key = _GetTypeKey(type);

            if (_configs.TryGetValue(key, out var oldConfig))
            {
                Logo.Error("Duplicate config: oldConfig={0}, newConfig={1}", oldConfig, config);
                return false;
            }

            _configs.Add(key, config);
            return true;
        }

        public Config GetConfig(Type configType)
        {
            if (null != configType)
            {
                var key = _GetTypeKey(configType);
                _configs.TryIndexValue(key, out var config);

                return config;
            }

            return null;
        }

        public bool RemoveConfig(Type configType)
        {
            if (null != configType)
            {
                var key = _GetTypeKey(configType);
                var removed = _configs.Remove(key);
                return removed;
            }

            return false;
        }

        public int GetConfigCount()
        {
            return _configs.Count;
        }

        public void Clear()
        {
            _configs.Clear();
        }

        internal void _TrimExcess()
        {
            _configs.TrimExcess();
        }

        public SortedTable<string, Config>.ValueList GetAllConfigs()
        {
            return _configs.Values;
        }

        private string _GetTypeKey(Type type)
        {
            return type.FullName;
        }

        public override string ToString()
        {
            var sbText = new System.Text.StringBuilder();

            foreach (var pair in _configs)
            {
                var typeName = pair.Key;
                sbText.Append(typeName);
//				sbText.Append("---");

//				sbText.Append(pair.Value.ToString());
                sbText.AppendLine();
            }

            var text = sbText.ToString();
            return text;
        }

        private readonly SortedTable<string, Config> _configs = new();
    }
}