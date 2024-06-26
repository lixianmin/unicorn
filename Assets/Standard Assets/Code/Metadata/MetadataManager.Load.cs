﻿/********************************************************************
created:    2015-10-27
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.IO;
using Unicorn;
using Unicorn.Collections;

namespace Metadata
{
    partial class MetadataManager
    {
        public LoadAid LoadRawStream(Stream stream, bool createImmediately = false)
        {
            var aid = GetLoadAid();

            if (null != stream)
            {
                var reader = MetaFactory.CreateChunkReader(stream);
                aid.Load(reader, false, out _metadataVersion);

                if (createImmediately)
                {
                    var iter = aid.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        if (iter.Value is SortedTable<int, LoadAid.NodeValue> section)
                        {
                            foreach (var (key, val) in section)
                            {
                                _CreateOneMetadata(aid, key, val);
                            }
                        }
                    }
                }
            }

            return aid;
        }

        private void _CreateOneMetadata(LoadAid aid, int key, LoadAid.NodeValue val)
        {
            aid.Seek(val);

            var metadata = MetaTools.Load(aid, null);
            var isAdded = AddMetadata(metadata);
            if (!isAdded)
            {
                Logo.Error("Invalid metadata, key={0}, metadata={1}", key, metadata);
            }
        }

        public void LoadIncrementStream(Stream stream)
        {
            var aid = GetLoadAid();
            if (stream is { CanRead: true, Length: > 0 } && null != aid)
            {
                var reader = MetaFactory.CreateChunkReader(stream);
                const bool isIncrement = true;
                aid.Load(reader, isIncrement, out _metadataVersion);
            }
        }

        public void LoadLocaleTextStream(Stream stream)
        {
            LocaleTextManager.It.Load(stream);
        }

        internal void AddMetadata(XmlMetadata metadata)
        {
            if (null == metadata)
            {
                return;
            }

            var templates = metadata.Templates;
            var templateCount = templates.Count;
            if (templateCount > 0)
            {
                for (var i = 0; i < templateCount; ++i)
                {
                    _templateManager.AddTemplate(templates[i]);
                }
            }

            var configs = metadata.Configs;
            var configCount = configs.Count;
            if (configCount > 0)
            {
                for (var i = 0; i < configCount; ++i)
                {
                    _configManager.AddConfig(configs[i]);
                }
            }
        }

        internal bool AddMetadata(IMetadata metadata)
        {
            if (null == metadata)
            {
                return false;
            }

            if (metadata is Template template)
            {
                _templateManager.AddTemplate(template);
                return true;
            }

            if (metadata is Config config)
            {
                _configManager.AddConfig(config);
                return true;
            }

            return false;
        }
    }
}