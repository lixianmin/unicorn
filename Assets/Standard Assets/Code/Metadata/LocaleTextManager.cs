
/********************************************************************
created:    2017-07-15
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unicorn;
using Unicorn.IO;

namespace Metadata
{
    public class LocaleTextManager
	{
        public LocaleText ReadLocaleText (IOctetsReader reader)
        {
            LocaleText ret = new LocaleText { text = string.Empty };

            if (null != reader)
            {
                int index = (int) reader.ReadUInt32 ();
                var isFullMode = reader.ReadBoolean();
                if (isFullMode)
                {
                    ret.guid = reader.ReadString();
                }

                var localeTexts = _localeTexts;
                if (index >= 0 && index < localeTexts.Length)
                {
                    ret.text = localeTexts[index];
                }
            }

            return ret;
        }

        internal void WriteLocaleText (IOctetsWriter writer, LocaleText text, bool isFullMode)
        {
            if (null == writer)
            {
                return;
            }

            var guid = text.GetGUID();
            uint localeIndex = GetLocaleIndex(guid);
            writer.Write(localeIndex);
            writer.Write(isFullMode);

            if (isFullMode)
            {
                writer.Write(guid);
            }
        }

        internal void Load (Stream stream)
        {
            if (null != stream && stream.CanRead && stream.Length > 0)
            {
                try
                {
                    var reader = new BinaryReader(stream);
                    _LoadLocaleTexts(reader);
                    _LoadLocaleIndices(reader);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }
        }

        private void _LoadLocaleTexts (BinaryReader reader)
        {
            var count = reader.ReadInt32();

            var localeTexts = new string[count];
            for (int i= 0; i< count; ++i)
            {
                localeTexts[i] = reader.ReadString();
            }

            _SetLocaleTexts(localeTexts);
        }

        private void _LoadLocaleIndices (BinaryReader reader)
        {
            var count = reader.ReadInt32();
            if (count > 0)
            {
                var localeIndices = _localeIndices;
                if (null != localeIndices)
                {
                    localeIndices.Clear();
                }
                else
                {
                    localeIndices = new Dictionary<string, uint>();
                    _SetLocaleIndices(localeIndices);
                }

                for (int i= 0; i< count; ++i)
                {
                    string guid = reader.ReadString();
                    uint index = reader.ReadUInt32();
                    localeIndices[guid] = index;
                }
            }
        }

        public void Save (Stream stream, bool isFullMode)
        {
            if (null != stream && stream.CanWrite)
            {
                try
                {
                    var writer = new BinaryWriter(stream);
                    _SaveLocaleTexts(writer);
                    _SaveLocaleIndices(writer, isFullMode);
                    writer.Flush();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }
        }

        private void _SaveLocaleTexts (BinaryWriter writer)
        {
            var localeTexts = _localeTexts;
            var count = localeTexts.Length;
            writer.Write(count);

            for (int i= 0; i< count; ++i)
            {
                writer.Write(localeTexts[i]);
            }
        }

        private void _SaveLocaleIndices (BinaryWriter writer, bool isFullMode)
        {
            var localeIndices = _localeIndices;
            var count = isFullMode && null != localeIndices ? localeIndices.Count : 0;

            writer.Write(count);
            if (count > 0)
            {
                var iter = localeIndices.GetEnumerator();
                while (iter.MoveNext())
                {
                    var pair = iter.Current;
                    string guid = pair.Key;
                    uint index= pair.Value;

                    writer.Write(guid);
                    writer.Write(index);
                }
            }
        }

        public void AddLocaleTable (LocaleTable localeTable)
        {
            if (null == localeTable)
            {
                return;
            }

            _AddLocaleIndicesToLocaleTable(localeTable);

            var count = localeTable.Count;
            var guidArray = new string[count];
            var textArray = new string[count];
            var index = 0;

            var iter = localeTable.GetEnumerator();
            while (iter.MoveNext())
            {
                guidArray[index] = iter.Key as string;
                textArray[index] = iter.Value as string;

                ++index;
            }

            Array.Sort(guidArray, textArray, ReversedStringComparer.Instance);
            _SetLocaleTexts(textArray);

            var localeIndices = new Dictionary<string, uint>(count);
            _SetLocaleIndices(localeIndices);

            for (uint i= 0; i< count; ++i)
            {
                var guid = guidArray[i]; // from entry.Key, so can not be null.
                localeIndices[guid] = i;
            }
        }

        private void _AddLocaleIndicesToLocaleTable (LocaleTable localeTable)
        {
            var localeIndices = _localeIndices;
            if (null == localeIndices)
            {
                return;
            }

            var iter = localeIndices.GetEnumerator();
            while (iter.MoveNext())
            {
                var pair = iter.Current;
                var guid = pair.Key;
                var text = _localeTexts[pair.Value];

                localeTable.Add(guid, text, true);
            }
        }

        private void _SetLocaleTexts (string[] localeTexts)
        {
            _localeTexts = localeTexts ?? EmptyArray<string>.Instance;
        }

        private void _SetLocaleIndices (Dictionary<string, uint> localeIndices)
        {
            _localeIndices = localeIndices ?? new Dictionary<string, uint>();
        }

        internal uint GetLocaleIndex (string guid)
        {
            var localeIndices = _localeIndices;
            uint index = 0;
            if (null != localeIndices && !string.IsNullOrEmpty(guid))
            {
                if (!localeIndices.TryGetValue(guid, out index))
                {
                    Console.Error.WriteLine("Invalid guid={0}, _localeIndices.Count={1}"
                        , guid, localeIndices.Count.ToString());
                }
            }

            return index;
        }

        internal void Clear ()
        {
            _SetLocaleTexts(EmptyArray<string>.Instance);
            _SetLocaleIndices(null);
        }

        public bool Contains (string guid)
        {
            var ret = null != guid && null != _localeIndices && _localeIndices.ContainsKey(guid);
            return ret;
        }

        public int GetCount ()
        {
            return _localeTexts.Length;
        }

        public static LocaleTextManager Instance = new LocaleTextManager();

        private string[] _localeTexts = EmptyArray<string>.Instance;
        private Dictionary<string, uint> _localeIndices;
	}
}