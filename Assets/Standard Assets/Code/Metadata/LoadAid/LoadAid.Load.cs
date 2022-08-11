
/********************************************************************
created:    2015-10-23
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.IO;
using System.Collections;
using Unicorn;
using Unicorn.IO;
using Unicorn.Collections;

namespace Metadata
{
	partial class LoadAid
	{
		internal void Load (OctetsReader reader, bool isIncrement, out int metadataVersion)
		{
			if (null == reader)
			{
				metadataVersion = 0;
				return;
			}

            var catalog = _catalog;
			if (!isIncrement)
			{
				catalog.Clear();
			}

			metadataVersion = reader.ReadInt32();

			if (isIncrement)
			{
                var incrementCatalog = new Hashtable();
				var flags = NodeFlags.Increament;
				_LoadCatalog(reader, incrementCatalog, flags);
                _MergeCatalog(catalog, incrementCatalog);
			}
			else
			{
				var flags = NodeFlags.None;
                _LoadCatalog(reader, catalog, flags);
			}

            _LoadTexts(reader, isIncrement);
            _LoadLayout(reader);
			_LoadAndRemoveData(reader, catalog);

			_SetReader(reader, isIncrement);
		}

		private void _SetReader (OctetsReader reader, bool isIncrement)
		{
			if (isIncrement)
			{
				_incrementReader = reader;
				_incrementReaderSeekOffset = (int) reader.BaseStream.Position;
			}
			else
			{
				_rawReader = reader;
				_rawReaderSeekOffset = (int) reader.BaseStream.Position;
			}
		}

		private static void _LoadCatalog (OctetsReader headReader, Hashtable catalog, NodeFlags flags)
		{
            var chapterCount  = headReader.ReadInt32();

			for (int i= 0; i< chapterCount; ++i)
			{
                string typeName = headReader.ReadString();
                var sectionCount = headReader.ReadInt32();

                var section = new SortedTable<int, NodeValue>(sectionCount);
                catalog[typeName] = section;

                for (int j= 0; j< sectionCount; ++j)
                {
                    int id = headReader.ReadInt32();
                    int offset  = headReader.ReadInt32();

                    var key = id;
                    var val = new NodeValue { offset = offset, flags = flags };
                    section._Append(key, val);
                }

                section._Sort();
			}
		}

        private void _MergeCatalog (Hashtable baseCatalog, Hashtable incrementCatalog)
        {
            var iter = incrementCatalog.GetEnumerator();
            while (iter.MoveNext())
            {
                var typeName = iter.Key as string;
                var incrementSection = iter.Value as SortedTable<int, NodeValue>;
                var baseSection = baseCatalog[typeName] as SortedTable<int, NodeValue>;

                if (null == baseSection)
                {
                    baseCatalog[typeName] = incrementSection;
                }
                else
                {
                    baseSection.Merge(incrementSection);
                }
            }
        }

		private void _LoadTexts (OctetsReader headReader, bool isIncrement)
		{
			var texts = _LoadTextsFromBinary(headReader);
			if (isIncrement)
			{
				var rawTexts = GetTexts();
				Array.Copy(rawTexts, texts, rawTexts.Length);
			}

			_texts = texts;
		}

        private void _LoadLayout (OctetsReader headReader)
        {
            int count = headReader.ReadUInt16();
            for (int i= 0; i< count; ++i)
            {
                var typeIndex = headReader.ReadUInt32();
                var typeName = _texts[typeIndex];

                int fieldCount = headReader.ReadUInt16();
                var layout = headReader.ReadBytes(fieldCount);

                var creator = MetaFactory.GetMetaCreator(typeName);
                if (null != creator)
                {
                    creator.SetLayout(layout);
                }
            }
        }

        private static void _LoadAndRemoveData (OctetsReader headReader, Hashtable catalog)
		{
			var removedCount = headReader.ReadInt32();

			for (int i= 0; i< removedCount; ++i)
			{
                string typeName = headReader.ReadString();
				int id = headReader.ReadInt32();

                var section = catalog[typeName] as SortedTable<int, NodeValue>;
                if (null != section)
                {
                    section.Remove(id);
                }
			}
		}

		private static string[] _LoadTextsFromBinary (BinaryReader reader)
		{
			var arrayLength = reader.ReadInt32();
			var itemCount = reader.ReadInt32();
			var texts = new string[arrayLength];

			for (int i= 0; i< itemCount; ++i)
			{
				var text  = reader.ReadString();
				var index = reader.ReadInt32();

				texts[index] = text;
			}

			return texts;
		}
	}
}
