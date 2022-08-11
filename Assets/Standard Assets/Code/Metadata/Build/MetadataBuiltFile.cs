
/********************************************************************
created:    2015-10-30
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Unicorn;
using Unicorn.IO;

namespace Metadata.Build
{
	using EntryTable = System.Collections.Hashtable;
	using DataItemTable = System.Collections.Hashtable;

    public class MetadataBuiltFile
    {
		public MetadataBuiltFile ()
		{

		}

		public IEnumerable<IMetadata> EnumerateMetadata ()
		{
			foreach (DataItem item in _dataItems.Values)
			{
				var metadata = item.GetMetadata();
				yield return metadata;
			}
		}

        public int GetMetadataVersion ()
        {
            return _metadataVerison;
        }

		public void Clear ()
		{
            foreach (var filepath in Kernel.walk(_GetDataFolder(), "*"))
			{
                FileTools.DeleteSafely(filepath);
			}
		}

        public bool LoadLocaleText ()
        {
            var filepath = _GetBuiltFilePath();
            if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath))
            {
                return false;
            }

            using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(stream))
            {
                var version = reader.ReadInt32();
                if (version != _currentVersion)
                {
                    return false;
                }

                LocaleTextManager.Instance.Load(stream);
                return true;
            }
        }

		public void Build ()
		{
			var filepath = _GetBuiltFilePath();
			Console.WriteLine("[Build()] builtFilePath={0}", filepath);

            _localeTable.Clear();
            LocaleText.OnDeserialize += _OnDeserializeLocaleText;

			var hasInvalidTypes = _LoadAll(filepath, _entries, _dataItems);
			foreach (string key in _entries.Keys)
			{
				_lastEntryXmlPaths.Add(key);
			}

            var metadataRoot = _GetMetadataRoot();
            _metadataVerison = SVNTools.GetRevision(metadataRoot);

            var xmlFiles = Kernel.walk(metadataRoot, "*.xml");
			
			var title = "Building *.xml metadata";
            ScanTools.ScanAll(title, xmlFiles, _OnBuildOneXmlFile);
            LocaleText.OnDeserialize -= _OnDeserializeLocaleText;
            LocaleTextManager.Instance.AddLocaleTable(_localeTable);
            Console.WriteLine("--> AddLocaleTable() done");

			if (_sbBuiltXmlPaths.Length > 0)
			{
				Console.WriteLine("--> build *.xml metadata, builtXmlPaths={0}", _sbBuiltXmlPaths);
			}

			var hasDeleted = _RemoveDeletedEntries();

			if (hasInvalidTypes || _xmlFileChanged || hasDeleted)
			{
				_SaveAll(filepath, _entries, _dataItems);
			}

			Console.WriteLine("--> build *.xml metadata done, filepath={0}, _entries.Count={1}, _dataItems.Count={2}"
			                  , filepath, _entries.Count, _dataItems.Count);
		}

        private void _OnDeserializeLocaleText (LocaleText localeText)
        {
            var guid = localeText.GetGUID();
            _localeTable.Add(guid, localeText.text, true);
        }

        private static string _GetMetadataRoot ()
        {
            var type = TypeTools.SearchType("Unicorn.UnicornManifest");
            if (null == type)
            {
                Console.Error.WriteLine("type of Unicorn.UnicornManifest is null");
                return string.Empty;
            }

            System.Func<string> lpfnGetMetadataRoot;
            TypeTools.CreateDelegate(type, "GetMetadataRoot", out lpfnGetMetadataRoot);
            if (null == lpfnGetMetadataRoot)
            {
                Console.Error.WriteLine("lpfnGetMetadataRoot is null");
                return string.Empty;
            }

            var metadataRoot = lpfnGetMetadataRoot();
            return metadataRoot;
        }

		private void _OnBuildOneXmlFile (string xmlPath)
		{
			var newEntry = EntryItem.Create(xmlPath);
			if (null == newEntry)
			{
				Console.Error.WriteLine("newEntry=null, xmlPath={0}", xmlPath);
				return;
			}
			
			var lastEntry = _entries[xmlPath] as EntryItem;
			if (null != lastEntry)
			{
				_lastEntryXmlPaths.Remove(xmlPath);
			}

			if (newEntry.TestEquals(lastEntry))
			{
				return;
			}
			
			var rawMetadata = XmlMetadata.Deserialize(xmlPath);
			if (null != rawMetadata)
			{
				// remove all old metadata.
				_RemoveDataItems(lastEntry, _dataItems);

				// add new templates.
				string typeName = null;
				foreach (var template in rawMetadata.Templates)
				{
                    if (null == template)
                    {
                        Console.Error.WriteLine("Invalid file format for template=null, xmlPath={0}", xmlPath);
                        continue;
                    }

					typeName = template.GetType().Name;
					var itemName = typeName + " " + template.id;
					var item = DataItem.Create(template);

					newEntry.AddDataItemName(itemName);
					_dataItems[itemName] = item;
				}

				// add new config.
				foreach (var config in rawMetadata.Configs)
				{
                    if (null == config)
                    {
                        Console.Error.WriteLine("Invalid file format for config=null, xmlPath={0}", xmlPath);
                        continue;
                    }

					typeName = config.GetType().Name;
					var itemName = typeName + " 0506";
					var item = DataItem.Create(config);

					newEntry.AddDataItemName(itemName);
					_dataItems[itemName] = item;
				}

				newEntry.SetTypeName(typeName);
				_entries[xmlPath] = newEntry;
				_xmlFileChanged = true;

				_sbBuiltXmlPaths.Append(xmlPath);
				_sbBuiltXmlPaths.AppendLine();
			}
			else
			{
                Console.Error.WriteLine("rawMetdata=null, this xml file may not be used any more, xmlPath={0}", xmlPath);
			}
		}

		private bool _RemoveDeletedEntries ()
		{
			var count = _lastEntryXmlPaths.Count;
			var hasDeleted = count > 0;
			if (hasDeleted)
			{
				var iter = _lastEntryXmlPaths.GetEnumerator();
				while (iter.MoveNext())
				{
					var deletedXmlPath = iter.Current;
					var deletedEntry = _entries[deletedXmlPath] as EntryItem;

					_RemoveDataItems(deletedEntry, _dataItems);
				}

				foreach (string xmlPath in new ArrayList(_entries.Keys))
				{
					if (_lastEntryXmlPaths.Contains(xmlPath))
					{
						_entries.Remove(xmlPath);
					}
				}

				_lastEntryXmlPaths.Clear();
			}

			return hasDeleted;
		}

		private static void _RemoveDataItems (EntryItem entry, DataItemTable dataItems)
		{
			// remove all old metadata.
			if (null == entry)
			{
				return;
			}

			var itemNames = entry.GetDataItemNames();
			if (null != itemNames)
			{
				foreach (string itemName in new ArrayList(dataItems.Keys))
				{
					if (itemNames.Contains(itemName))
					{
						dataItems.Remove(itemName);
					}
				}
			}
		}

		private static bool _LoadAll (string filepath, EntryTable entries, DataItemTable dataItems)
		{
			if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath))
			{
				return true;
			}

			using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
			using (var reader = new BinaryReader(stream))
			{
				var version = reader.ReadInt32();
				if (version != _currentVersion)
				{
					return true;
				}

                LocaleTextManager.Instance.Load(stream);
				_LoadEntries (reader, entries);
				var invalidTypeNames = _LoadDataItems (reader, dataItems);

				var hasInvalidTypes = invalidTypeNames.Count > 0;
				if (hasInvalidTypes)
				{
					foreach (DictionaryEntry pair in new ArrayList(entries))
					{
						var entry = pair.Value as EntryItem;
						if (invalidTypeNames.Contains(entry.GetTypeName()))
						{
							var xmlPath = pair.Key as string;
							entries.Remove(xmlPath);
						}
					}
				}

				var isEntriesRemoved = _RemoveLostEntries (entries, dataItems);
				var needSave = hasInvalidTypes || isEntriesRemoved;
				return needSave;
			}
		}

		private static bool _RemoveLostEntries (EntryTable entries, DataItemTable dataItems)
		{
			var sbText = new StringBuilder();

			var itemNameToPaths = new Hashtable();
			foreach (DictionaryEntry pair in entries) 
			{
				var entry = pair.Value as EntryItem;
				var dataItemNames = entry.GetDataItemNames ();
				if (null == dataItemNames)
				{
					continue;
				}

				foreach (var itemName in dataItemNames)
				{
					var newFilePath = pair.Key;
					var oldFilePath = itemNameToPaths[itemName] as string;
					if (null == oldFilePath)
					{
						itemNameToPaths.Add (itemName, newFilePath);
					}
					else
					{
						sbText.AppendFormat ("Duplicated filepath, itemName={0}, newFilePath={1}, oldFilePath={2}\n\n", itemName, newFilePath, oldFilePath);
					}
				}
			}

			if (sbText.Length > 0)
			{
                Console.Error.WriteLine(sbText);
			}

			var isEntriesRemoved = false;
			foreach (DictionaryEntry pair in itemNameToPaths) 
			{
				var itemName = pair.Key;
				if (!dataItems.ContainsKey (itemName))
				{
					var filepath = pair.Value as string;
					entries.Remove (filepath);
					isEntriesRemoved = true;
				}
			}

			return isEntriesRemoved;
		}

		private static void _SaveAll (string filepath, EntryTable entries, DataItemTable dataItems)
		{
			if (string.IsNullOrEmpty(filepath))
			{
				return;
			}

			var tempFilePath = filepath + ".tmp.0506";
			FileStream stream = null;
			BinaryWriter writer = null;

			try 
			{
				stream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write);
				writer = new BinaryWriter(stream);

				writer.Write(_currentVersion);
                LocaleTextManager.Instance.Save(stream, true);
				_SaveEntries(writer, entries);
				_SaveDataItems(writer, dataItems);

                if (null != writer)
                {
                    writer.Close();
                    writer = null;
                }

                if (null != stream)
                {
                    stream.Close();
                    stream = null;
                }

				FileTools.Overwrite(tempFilePath, filepath);
			}
			catch (Exception)
			{
                if (null != writer)
                {
                    writer.Close();
                    writer = null;
                }

                if (null != stream)
                {
                    stream.Close();
                    stream = null;
                }
			}
		}

		private static HashSet<string> _LoadDataItems (BinaryReader masterReader, DataItemTable dataItems)
		{
			dataItems.Clear();
			var invalidTypeNames = new HashSet<string>();

			var count = masterReader.ReadInt32();
			for (int i= 0; i< count; ++i)
			{
				var itemName   = masterReader.ReadString();
				var itemBuffer = _LoadBytes(masterReader);

				var texts = itemName.Split();
				var typeName = texts[0];

				// we must remove all invalid files, because it may not be overwrited by XmlMetadata.Deserialize().
				if (invalidTypeNames.Contains(typeName))
				{
					continue;
				}

				try 
				{
					var item = DataItem.Create(itemBuffer);
					if (null != item)
					{
						dataItems.Add(itemName, item);
					}
					else 
					{
						invalidTypeNames.Add(typeName);
					}
				}
				catch (Exception)
				{
					invalidTypeNames.Add(typeName);
				}
			}

			return invalidTypeNames;
		}

		private static void _SaveDataItems (BinaryWriter writer, DataItemTable dataItems)
		{
			var count = dataItems.Count;
			writer.Write(count);

			foreach (DictionaryEntry pair in dataItems)
			{
				var itemName = pair.Key as string;
				var item = pair.Value as DataItem;

				writer.Write(itemName);
				item.Save(writer);
			}
		}

		private static void _LoadEntries (BinaryReader reader, EntryTable entries)
		{
			entries.Clear();
			
			var count = reader.ReadInt32();
			for (int i= 0; i< count; ++i)
			{
				var fullpath = reader.ReadString();
				
				var entry = new EntryItem();
				entry.Load(reader);
				entries.Add(fullpath, entry);
			}
		}

		private static void _SaveEntries (BinaryWriter writer, EntryTable entries)
		{
			var count = entries.Count;
			writer.Write(count);
			
			foreach (DictionaryEntry pair in entries)
			{
				var filename = pair.Key as string;
				var entry = pair.Value as EntryItem;
				
				writer.Write(filename);
				entry.Save(writer);
			}
		}
		
		private static byte[] _LoadBytes (BinaryReader masterReader)
		{
			var bufferLength = masterReader.ReadInt32();
			var buffer = new byte[bufferLength];
			masterReader.BaseStream.Read(buffer, 0, bufferLength);

			return buffer;
		}

		private static void _SaveBytes (BinaryWriter writer, byte[] buffer)
		{
			int bufferLength = buffer.Length;
			writer.Write(bufferLength);
			writer.BaseStream.Write(buffer, 0, bufferLength);
		}

		private static string _GetDataFolder ()
		{
			if (null == _dataFolder)
			{
                _dataFolder = Path.GetTempPath() + "/metadata";
                Kernel.makedirs(_dataFolder);
			}
			
			return _dataFolder;
		}
		
		private string _GetBuiltFilePath ()
		{
			var fullpath = _GetDataFolder() + "/built-file.bf";
			return fullpath;
		}

		private static readonly int _currentVersion = 0x0318;
		private static string _dataFolder;

		private readonly EntryTable _entries = new EntryTable(2048);
		private readonly DataItemTable _dataItems = new DataItemTable(8192);
		private readonly HashSet<string> _lastEntryXmlPaths = new HashSet<string>();
		private readonly StringBuilder _sbBuiltXmlPaths = new StringBuilder();
		private bool _xmlFileChanged;
        private int _metadataVerison;

        private readonly LocaleTable _localeTable = new LocaleTable();
    }
}