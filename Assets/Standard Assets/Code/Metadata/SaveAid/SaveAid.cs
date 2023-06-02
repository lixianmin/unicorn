
/********************************************************************
created:    2015-01-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

// #define Unicorn_TEST

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Unicorn;
using Unicorn.IO;
using Unicorn.Collections;

using CatalogTable = Unicorn.Collections.SortedTable<string, Unicorn.Collections.SortedTable<int, int>>;

namespace Metadata
{
	public partial class SaveAid: IOctetsWriter
    {
		private enum CountType
		{
			Bool,
			Byte,
			SByte,
			Int16,
			UInt16,
			Int32,
			UInt32,
			Int64,
			UInt64,
			Float,
			Double,
			Vector2,
			Vector3,
			Vector4,
			String,
			Count,
		}

		public SaveAid (IncrementData incrementData)
		{
			if (null != incrementData)
			{
				_incrementData= incrementData;
				var baseTexts = incrementData.GetBaseTexts();

				if (null != baseTexts)
				{
					var count = baseTexts.Length;
					for (int index= 0; index< count; ++index)
					{
						var text = baseTexts[index];
                        _textTable.Add(text, index);
					}

					_indexGenerator = count;
					_baseIndex = count;
				}
			}
		}

        public void Save (Stream stream, MetadataManager manager, bool isFullMode)
        {
			if (null == stream || null == manager || !stream.CanWrite)
			{
				return;
			}

			var headStream = new MemoryStream(8*1024*1024);
			var bodyStream = new MemoryStream(16*1024*1024);
			var mode = OctetsMode.Compress;

			using (var headWriter = new OctetsWriter(headStream, mode))
			using (_bodyWriter = new OctetsWriter(bodyStream, mode))
			{
				headWriter.Write(manager.GetMetadataVersion());
				_bodyBoolPacker.AttachStream(_bodyWriter.BaseStream);

                var catalog = new CatalogTable();
                _SaveTemplates(catalog, manager.GetTemplateManager(), isFullMode);
                _SaveConfigs(catalog, manager.GetConfigManager(), isFullMode);
                _SaveCatalog(headWriter, catalog);

                var creators = _FetchNoEditorCreators();
                _AddMetadataTypesToTextTable(creators);
                _SaveTexts(headWriter);
                _SaveLayout(headWriter, creators);

 				_SaveRemovedMetadata(headWriter);

				headStream.WriteTo(stream);
				bodyStream.WriteTo(stream);

				// Console.Warning.WriteLine("nodes.count={0}, headSteam.length={1}, bodyStream.length={2}", nodes.Count, headStream.Length, bodyStream.Length);
				_bodyWriter.Write("Hey guys, welcome to our magic world, any questions please contact me by lixianmin@live.com");
			}
        }

		[System.Diagnostics.Conditional("Unicorn_TEST")]
		internal void PrintStatistics ()
		{
			var sbText = new System.Text.StringBuilder();
			if (null != _counter)
			{
				for (int i= 0; i< _counter.Length; ++i)
				{
					var type = (CountType) i;
					sbText.Append(type.ToString());
					sbText.Append("\t\t");
					sbText.Append(_counter[i]);
					sbText.Append("\t\t");
					
					var itemSize = 1;
					if (type == CountType.Int16 || type == CountType.UInt16)
					{
						itemSize = 2;
					}
					else if (type == CountType.Int32 || type == CountType.UInt32 
					         || type == CountType.Float)
					{
						itemSize = 4;
					}
					else if (type == CountType.Int64 || type == CountType.UInt64
					         || type == CountType.Double || type == CountType.Vector2)
					{
						itemSize = 8;
					}
					else if (type == CountType.Vector3)
					{
						itemSize = 12;
					}
					else if (type == CountType.Vector4)
					{
						itemSize = 16;
					}
					else if (type == CountType.String)
					{
						itemSize = 0;
					}
					
					var totalSize = _counter[i] * itemSize;
					sbText.Append(totalSize);
					sbText.AppendLine();
				}
			}

			var text = sbText.ToString();
			Logo.Info(text);
		}

        private static SortedTable<string, TemplateTable> _GetSortedTemplateTables (TemplateManager templateManager)
        {
            var tables = new SortedTable<string, TemplateTable>();
            foreach (var pair in templateManager.EnumerateTemplateTables())
            {
                var type = pair.Key;
                var table = pair.Value;
                tables._Append(type.FullName, table);
            }

            tables._Sort();
            return tables;
        }

        private void _SaveTemplates (CatalogTable catalog, TemplateManager templateManager, bool isFullMode)
		{
			var bodyStream  = _bodyWriter.BaseStream;
            var templateTables = _GetSortedTemplateTables (templateManager);

            foreach (var pair in templateTables)
            {
                var typeName = pair.Key;
                var section = new SortedTable<int, int>();
                catalog._Append(typeName, section);

                var table = pair.Value;
                foreach (var template in table.Values)
                {
                    var id = template.id;
                    var offset = (int) bodyStream.Position;
                    section._Append(id, offset);

                    _SaveOneTemplate(template, isFullMode);
                }

                section._Sort();
            }

            catalog._Sort();
		}

        private void _SaveOneTemplate (Template template, bool isFullMode)
        {
            var callback = template as IExportCallback;
            if (null != callback)
            {
                try { callback.OnExporting(); } catch { }
            }

            MetaTools.Save(this, template, isFullMode);
            _bodyBoolPacker.Flush();

            if (null != callback)
            {
                try { callback.OnExported(); } catch { }
            }
        }

        private void _SaveConfigs (CatalogTable catalog, ConfigManager configManager, bool isFullMode)
		{
			var configs = configManager.GetAllConfigs();
			var contentStream = _bodyWriter.BaseStream;

			foreach (var config in configs)
			{
				if (null == config)
				{
                    continue;
				}

                var section = new SortedTable<int, int>(1);
                var id = 0;
                var offset = (int) contentStream.Position;
                section.Add(id, offset);

                var type = config.GetType();
                var typeName = type.FullName;
                catalog._Append(typeName, section);
				
                MetaTools.Save(this, config, isFullMode);
				_bodyBoolPacker.Flush();
            }

            catalog._Sort();
		}

        private void _SaveCatalog (BinaryWriter writer, CatalogTable catalog)
		{
			var count = catalog.Count;
			writer.Write(count);

			foreach (var pair in catalog)
			{
                var typeName = pair.Key;
				writer.Write(typeName);

                var section = pair.Value;
                writer.Write(section.Count);
                foreach (var p2 in section)
                {
                    var id = p2.Key;
                    var offset = p2.Value;
                    writer.Write(id);
                    writer.Write(offset);
                }
			}
		}
		
		private IEnumerable<KeyValuePair<string, int>> _EnumerateSavableTexts ()
		{
			var iter = _textTable.GetEnumerator();
			while (iter.MoveNext())
			{
				var pair = iter.Current;
				var index= pair.Value;
				
				if (index >= _baseIndex)
				{
					yield return pair;
				}
			}
		}

        private MetaCreator[] _FetchNoEditorCreators ()
        {
            var creators = (from creator in MetaFactory.EnumerateMetaCreators()
                where !creator.IsEditorOnlyCreator()
                orderby creator.GetMetadataType().FullName
                select creator).ToArray();
            
            return creators;
        }

        private void _SaveLayout (BinaryWriter writer, MetaCreator[] creators)
        {
            var count = (ushort) creators.Length;
            writer.Write(count);

            foreach (var creator in creators)
            {
                var metadataType = creator.GetMetadataType();
                var typeName = metadataType.FullName;
                int typeIndex = _textTable[typeName];

                var layout = creator.GetLayout();
                ushort fieldCount = (ushort) layout.Length;

                writer.Write((uint) typeIndex);
                writer.Write(fieldCount);
                writer.Write(layout);
            }
        }

		private void _SaveTexts (BinaryWriter writer)
		{
			var arrayLength = _textTable.Count;
			writer.Write(arrayLength);

			var itemCount = System.Linq.Enumerable.Count(_EnumerateSavableTexts());
			writer.Write(itemCount);

			foreach (var pair in _EnumerateSavableTexts())
			{
				var text = pair.Key;
				var index= pair.Value;

				writer.Write(text);
				writer.Write(index);
			}
		}

		private void _SaveRemovedMetadata (BinaryWriter writer)
		{
			var removedData = null != _incrementData ? _incrementData.GetRemovedMetadatas() : null;
			var removedCount = null != removedData ? removedData.Count : 0;

			writer.Write(removedCount);
			if (null != removedData)
			{
				for (int i=0; i< removedCount; ++i)
				{
                    var key = removedData[i] as RemovedData;
                    writer.Write(key.typeName);
					writer.Write(key.id);
				}
			}
		}

		public void	Write (string text)
		{
            var index = _FetchTextIndex(text);
            _bodyWriter.Write((uint) index);
			_IncrementCounter(CountType.String);
		}

        private int _FetchTextIndex (string text)
        {
            text = text ?? string.Empty;

            int index;
            if (!_textTable.TryGetValue(text, out index))
            {
                index = _indexGenerator++;
                _textTable.Add(text, index);
            }

            return index;
        }

        private void _AddMetadataTypesToTextTable (MetaCreator[] creators)
        {
            foreach (var creator in creators)
            {
                var metadataType = creator.GetMetadataType();
                var typeName = metadataType.FullName;
                _FetchTextIndex(typeName);
            }
        }

		[System.Diagnostics.Conditional("Unicorn_TEST")]
		private void _IncrementCounter (CountType type)
		{
			var index = (int) type;

			if (null == _counter)
			{
				_counter = new int[(int) CountType.Count];
			}

			++_counter[index];
		}

		public void	Write (bool val)
		{
			_bodyBoolPacker.Write(val);
			_IncrementCounter(CountType.Bool);
		}
		
		public void	Write (byte v)		{ _bodyWriter.Write(v); _IncrementCounter(CountType.Byte); }

		public void	Write (sbyte v)		{ _bodyWriter.Write(v); _IncrementCounter(CountType.SByte); }
		
		public void	Write (short v)		{ _bodyWriter.Write(v); _IncrementCounter(CountType.Int16); }
		
		public void	Write (ushort v)	{ _bodyWriter.Write(v); _IncrementCounter(CountType.UInt16); }

		public void	Write (int v)		{ _bodyWriter.Write(v); _IncrementCounter(CountType.Int32); }
		
		public void	Write (uint v)		{ _bodyWriter.Write(v); _IncrementCounter(CountType.UInt32); }
		
		public void	Write (long v)		{ _bodyWriter.Write(v); _IncrementCounter(CountType.Int64); }
		
		public void	Write (ulong v)		{ _bodyWriter.Write(v); _IncrementCounter(CountType.UInt64); }
		
		public void	Write (float v)		{ _bodyWriter.Write(v); _IncrementCounter(CountType.Float); }
		
		public void	Write (double v)	{ _bodyWriter.Write(v); _IncrementCounter(CountType.Double); }

        public void	Write (float x, float y) { _bodyWriter.Write(x, y); _IncrementCounter(CountType.Vector2); }

        public void	Write (float x, float y, float z) { _bodyWriter.Write(x, y, z); _IncrementCounter(CountType.Vector3); }

        public void	Write (float x, float y, float z, float w) { _bodyWriter.Write(x, y, z, w); _IncrementCounter(CountType.Vector4); }

		private OctetsWriter _bodyWriter;
		private int[] _counter;

        private readonly Dictionary<string, int> _textTable = new Dictionary<string, int>();
        private int _indexGenerator;
		private int _baseIndex;	// for increment export.
		private IncrementData _incrementData;

		private readonly BoolPacker _bodyBoolPacker = new BoolPacker();
    }
}