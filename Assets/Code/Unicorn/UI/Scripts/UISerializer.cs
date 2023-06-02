
/********************************************************************
created:    2017-06-01
author:     lixianmin

	1. 将脚本挂在界面的prefab上试用；
	2. 右键点击脚本组件选择Serialize Prefab操作进行序列化；
	3. 进行序列化通过prefab名字与界面同名lua类的Layout表中定义的字段匹配进行；
	4. 序列化会将控件的数据保存在prefab资源中；

	注意：制作界面prefab节点的名字必须唯一；

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Unicorn.UI
{
    [DisallowMultipleComponent]
	public class UISerializer : MonoBehaviour
    {
        [Serializable]
        public class WidgetData : IComparable<WidgetData>
        {
            public int CompareTo (WidgetData rhs)
            {
                if (null == rhs)
                {
                    return 1;
                }

                var myName = name ?? string.Empty;
                var nameCompareResult = string.Compare(myName, rhs.name, StringComparison.Ordinal);
                if (nameCompareResult != 0)
                {
                    return nameCompareResult;
                }

                var myType = type ?? string.Empty;
                var typeCompareResult = string.Compare(myType, rhs.type, StringComparison.Ordinal);
                return typeCompareResult;
            }

            public override string ToString()
            {
                return $"[WidgetData] name={name}, type={type}";
            }

            public string name = string.Empty;
            public string type = string.Empty;  // 这里只能序列化字符串, 无法序列化System.Type

            public Component target;

            [HideInInspector] public string userdata;
        }

        private void Awake ()
        {
            _ResetLabelsFont();
            _scripts.Add(this, this);

            openWindowAnimation.SetEnabledEx(false);
            closeWindowAnimation.SetEnabledEx(false);
        }

        private void Destroy ()
        {
            _scripts.Remove(this);
        }

        public static void SetLocaleFonts (string[] rawFontNames, Font[] localeFonts)
        {
            if (null == rawFontNames || rawFontNames.Length == 0)
            {
                return;
            }

            if (null == localeFonts || localeFonts.Length == 0)
            {
                return;
            }

            if (rawFontNames.Length != localeFonts.Length)
            {
                return;
            }

            _rawFontNames = rawFontNames;
            _localeFonts = localeFonts;

            // refresh all script fonts.
            var iter = _scripts.GetEnumerator();
            while (iter.MoveNext())
            {
                var entry = (DictionaryEntry) iter.Current;
                var script = entry.Value as UISerializer;
                if (script != null)
                {
                    script._ResetLabelsFont();
                }
            }
        }

        private void _ResetLabelsFont ()
        {
            var count = _rawFontNames.Length;
            if (null == labels || labels.Length == 0 || count == 0 || _localeFonts.Length != count)
            {
                return;
            }

            var cachedFonts = new Font[count];
            foreach (var label in labels)
            {
                var nextFont = _GetFont(label.font, cachedFonts);
                if (null != nextFont)
                {
                    label.font = nextFont;
                }
                else
                {
                    Logo.Warn("nextFont=null, label={0}, label.font={1}, cachedFonts=[{2}]"
                        , label.text, label.font, ", ".JoinEx(cachedFonts));
                    
                }
            }
        }

        private static Font _GetFont (Font currentFont, Font[] cachedFonts)
        {
            var count = cachedFonts.Length;
            for (var i= 0; i< count; ++i)
            {
                var cached = cachedFonts[i];
                if (null != cached)
                {
                    if (currentFont == cached)
                    {
                        return _localeFonts[i];
                    }
                }
                else  if (currentFont.name == _rawFontNames[i])
                {
                    cachedFonts[i] = currentFont;
                    var retFont = _localeFonts[i];
                    return retFont;
                }
            }

            return null;
        }

        private void Reset ()
        {
            _SerializePrefab();
        }

        private void _SerializePrefab ()
        {
            if (null == _lpfnSerializePrefab)
            {
                var type = _GetUISerializerEditorType();
                var flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static;
                var method = type.GetMethod("SerializePrefab", flags);
                TypeTools.CreateDelegate(method, out _lpfnSerializePrefab);
            }

            _lpfnSerializePrefab(this);
        }

        private static Type _GetUISerializerEditorType ()
        {
            var type = TypeTools.SearchType("Unicorn.UI.UISerializerEditor");
            return type;
        }

        private static string[] _rawFontNames = Array.Empty<string>();
        private static Font[] _localeFonts;
        private static readonly Hashtable _scripts = new();

        private static Action<UISerializer> _lpfnSerializePrefab;

        public WidgetData[] widgets;
        
        public UIWindowAnimation openWindowAnimation;
        public UIWindowAnimation closeWindowAnimation;
        
        public Text[] labels;
    }
}