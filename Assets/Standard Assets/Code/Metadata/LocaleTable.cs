
/********************************************************************
created:    2017-07-25
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections;
using Unicorn;

namespace Metadata
{
    public class LocaleTable
	{
        public void Add (string guid, string text, bool checkDuplication)
        {
            if (null == guid)
            {
                return;
            }

            if (checkDuplication)
            {
                var last = _table[guid] as string;
                if (null == last)
                {
                    _table[guid] = text;
                }
                else if (last != text)
                {
                    Logo.Error("Duplicated localeTexts with the same guid='{0}', lastText='{1}', newText='{2}'", guid, last, text);
                }
            }
            else
            {
                _table[guid] = text;
            }
        }

        public void AddLacked(string guid, string text)
        {
            if (null != guid && null != text)
            {
                var last = _table[guid];
                if (null == last)
                {
                    _table[guid] = text;
                }
            }
        }

//        public string Get (string guid)
//        {
//            if (null != guid)
//            {
//                var text = _table[guid] as string;
//                return text;
//            }
//
//            return null;
//        }
//
        public void Clear ()
        {
            _table.Clear();
        }

        public IDictionaryEnumerator GetEnumerator ()
        {
            return _table.GetEnumerator();
        }

        public int Count => _table.Count;

        private readonly Hashtable _table = new();
	}
}