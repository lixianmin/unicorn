
/********************************************************************
created:    2015-10-24
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.IO;
using System.Collections.Generic;

namespace Metadata.Build
{
    internal class EntryItem
    {
		public static EntryItem Create (string filepath)
		{
			try 
			{
				if (File.Exists(filepath))
				{
					var fileInfo = new FileInfo(filepath);
					
					var entry = new EntryItem();
					entry._size = fileInfo.Length;
					entry._mtime = fileInfo.LastWriteTime.Ticks;

					return entry;
				}
			}
			catch (Exception)
			{
				
			}
			
			return null;
		}

		public void Load (BinaryReader reader)
		{
			_size  = reader.ReadInt64();
			_mtime = reader.ReadInt64();
			_typeName = reader.ReadString();
			_ReadMetadataNames(reader);
		}

		public void Save (BinaryWriter writer)
		{
			writer.Write(_size);
			writer.Write(_mtime);
			writer.Write(_typeName);
			_SaveMetadataNames(writer);
		}

		private void _ReadMetadataNames (BinaryReader reader)
		{
			var count = reader.ReadInt32();
			if (count > 0)
			{
				if (null == _dataItemNames)
				{
					_dataItemNames = new HashSet<string>();
				}
				else 
				{
					_dataItemNames.Clear();
				}
				
				for (int i= 0; i< count; ++i)
				{
					var name = reader.ReadString();
					_dataItemNames.Add(name);
				}
			}
		}

		private void _SaveMetadataNames (BinaryWriter writer)
		{
			int count = null != _dataItemNames ? _dataItemNames.Count : 0;
			writer.Write(count);
			if (count > 0)
			{
				foreach (string name in _dataItemNames)
				{
					writer.Write(name);
				}
			}
		}

		public void SetTypeName (string typeName)
		{
			_typeName = typeName ?? string.Empty;
		}

		public string GetTypeName ()
		{
			return _typeName;
		}

		public bool TestEquals (EntryItem other)
		{
			if (null != other && other._size == _size && other._mtime == _mtime)
			{
				return true;
			}

			return false;
		}

		public void AddDataItemName (string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return;
			}

			if (null == _dataItemNames)
			{
				_dataItemNames = new HashSet<string>();
			}

			_dataItemNames.Add(name);
		}

		public ICollection<string> GetDataItemNames ()
		{
			return _dataItemNames;
		}

		private long _size;
		private long _mtime;
		private string _typeName;

		private HashSet<string> _dataItemNames;
    }
}