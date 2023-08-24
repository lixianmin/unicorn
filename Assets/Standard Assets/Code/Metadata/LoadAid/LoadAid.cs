
/********************************************************************
created:    2015-01-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Unicorn;
using Unicorn.Collections;
using Unicorn.IO;

namespace Metadata
{
	public partial class LoadAid: IDisposable, IOctetsReader
    {
		internal LoadAid ()
		{

		}

		public void Dispose ()
		{
            if (null != _rawReader)
            {
                _rawReader.Close();
                _rawReader = null;
            }

            if (null != _incrementReader)
            {
                _incrementReader.Close();
                _incrementReader = null;
            }

			_currentReader = null;
			_catalog.Clear();
			_texts = null;
		}

		internal void Seek (NodeValue val)
		{
			var readerSeekOffset = 0;

			if ((val.flags & NodeFlags.Increament) == 0)
			{
				_currentReader 	= _rawReader;
				readerSeekOffset= _rawReaderSeekOffset;
			}
			else
			{
				_currentReader	= _incrementReader;
				readerSeekOffset= _incrementReaderSeekOffset;
			}

			if (null != _currentReader)
			{
				var stream = _currentReader.BaseStream;
				stream.Seek(val.offset + readerSeekOffset, SeekOrigin.Begin);
				
				_boolPacker.AttachStream(stream);
			}
			else
			{
				Logo.Error("[Seek()] _currentReader is null.");
			}
		}

		public bool Seek (string metadataType, int metadataId)
		{
            var section = _GetSection(metadataType);
            if (null != section)
            {
                NodeValue val;

                if (section.TryGetValue(metadataId, out val))
                {
                    Seek(val);
                    return true;
                }
            }

			return false;
		}

		public IEnumerable<int> EnumerateIDs (string typeName)
		{
            var section = _GetSection(typeName);
			
            if (null != section)
            {
                var keys = section.Keys;
                return keys;
            }
			
            return null;
		}

        internal void LoadTemplates (string typeName, TemplateTable table)
		{
            if (table.IsCompleted)
            {
                return;
            }

            var section = _GetSection(typeName);
            if (null == section)
            {
                return;
            }

            foreach (var (id, node) in section)
            {
	            var templateIndex = table.IndexOfKey(id);
                if (templateIndex < 0)
                {
                    Seek(node);
                    var metadata = MetaTools.Load(this, null);

                    var template = metadata as Template;
                    table._Append(id, template);
                }
            }

            table._Sort();
            table.IsCompleted = true;
		}

        private SortedTable<int, NodeValue> _GetSection (string typeName)
        {
            var section = _catalog[typeName] as SortedTable<int, NodeValue>;
            return section;
        }

		internal IDictionaryEnumerator GetEnumerator ()
		{
			return _catalog.GetEnumerator();
		}

		public string[] GetTexts ()
		{
			return _texts;
		}

		public string ReadString ()
		{
			uint index = _currentReader.ReadUInt32 ();
			string text = _texts[index];
			return text;
		}

		public bool	ReadBoolean ()
		{
			return _boolPacker.Read();
		}

		public byte		ReadByte ()		{ return _currentReader.ReadByte(); }

		public sbyte	ReadSByte ()	{ return _currentReader.ReadSByte(); }

		public short	ReadInt16 ()	{ return _currentReader.ReadInt16(); }

		public ushort	ReadUInt16 ()	{ return _currentReader.ReadUInt16(); }

		public int		ReadInt32 ()	{ return _currentReader.ReadInt32(); }

		public uint		ReadUInt32 ()	{ return _currentReader.ReadUInt32(); }
		
		public long 	ReadInt64 ()	{ return _currentReader.ReadInt64(); }

		public ulong 	ReadUInt64 ()	{ return _currentReader.ReadUInt64(); }

		public float 	ReadSingle ()	{ return _currentReader.ReadSingle(); }

		public double 	ReadDouble ()	{ return _currentReader.ReadDouble(); }

        public void     ReadVector (out float x, out float y) { _currentReader.ReadVector(out x, out y); }
		
        public void     ReadVector (out float x, out float y, out float z) { _currentReader.ReadVector(out x, out y, out z); }
		
        public void     ReadVector (out float x, out float y, out float z, out float w) { _currentReader.ReadVector(out x, out y, out z, out w); }
		
        // string -> SortedTable<int, NodeValue>
        private readonly Hashtable _catalog = new Hashtable();

		private OctetsReader	_currentReader;
		private OctetsReader	_rawReader;
		private int				_rawReaderSeekOffset;
		private OctetsReader 	_incrementReader;
		private int				_incrementReaderSeekOffset;

		private readonly BoolPacker	_boolPacker = new BoolPacker();

        private string[] _texts;
    }
}