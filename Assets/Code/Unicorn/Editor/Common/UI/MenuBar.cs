
/********************************************************************
created:    2014-09-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Unicorn
{
	public class MenuBar
	{
		private struct MenuItem
		{
			public string name;
			public GenericMenu menu;
			public Action handler;
		}

		public void AddMenu (string name, GenericMenu menu)
		{
			if (string.IsNullOrEmpty(name) || null == menu)
			{
				return;
			}

			var item  = new MenuItem { name = name, menu = menu };
            var count = _items.Count;
            for (int i= 0; i< count; ++i)
            {
                if (_items[i].name == name)
                {
                    _items[i] = item;
                    return;
                }
            }
            
            _items.Add(item);
		}

		public void AddButton (string name, Action handler)
		{
			if (string.IsNullOrEmpty(name) || null == handler)
			{
				return;
			}
			
			var item  = new MenuItem { name = name, handler = handler };
			var count = _items.Count;
			for (int i= 0; i< count; ++i)
			{
				if (_items[i].name == name)
				{
					_items[i] = item;
					return;
				}
			}
			
			_items.Add(item);
		}

		public void OnGUI ()
		{
            _CheckInitSkin();
            _OnGUI_MenuBar();
		}

        private void _CheckInitSkin()
        {
            if (null == _skin)
            {
                _skin = GameObject.Instantiate(GUI.skin) as GUISkin;
				var skin = _skin;

                var buttonStyle = _skin.GetStyle("Button");
                buttonStyle.normal.background = null;
				buttonStyle.alignment = TextAnchor.MiddleLeft;

				var labelStyle = skin.GetStyle("Label");
				labelStyle.alignment = TextAnchor.MiddleLeft;

				var textFieldStyle = skin.GetStyle("TextField");
				textFieldStyle.alignment = TextAnchor.MiddleLeft;
                
				_menuItemWidth = 50.0f;

                float minWidth, maxWidth;
                for (int i= 0; i< _items.Count; ++i)
                {
                    var item = _items[i];
                    buttonStyle.CalcMinMaxWidth(new GUIContent(item.name), out minWidth, out maxWidth);
                    _menuItemWidth = Mathf.Max(_menuItemWidth, minWidth);
                }
                
                _menuItemWidthOption = GUILayout.Width(_menuItemWidth);
            }
        }

        private void _OnGUI_MenuBar ()
        {
            var lastSkin = GUI.skin;
            GUI.skin = _skin;

            {
				GUI.Box(new Rect(0, 0, EditorTools.GetScreenWidth(), _menuBarHeight), string.Empty);
                
                GUILayout.BeginHorizontal();
                {
                    const int leftButton = 0;
                    var count = _items.Count;
                    
                    for (int i= 0; i< count; ++i)
                    {
                        var item = _items[i];
                        
                        if (GUILayout.Button(item.name, _menuItemWidthOption)
                           && Event.current.button == leftButton)
                        {
							if (null != item.menu)
							{
								var width = _menuItemWidth + 4;
								var area = new Rect(width * i + 4, 0, width, _menuBarHeight);
								item.menu.DropDown(area);
							}
							else if (null != item.handler)
							{
								item.handler();
							}
                        }
                    }

					if (null != _callback)
					{
						var left = _menuItemWidth * count + 4;
						var area = new Rect (left, 0.0f, EditorTools.GetScreenWidth() - left, _menuBarHeight);
						_callback(area);
					}
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10.0f);
            }

            GUI.skin = lastSkin;
        }

		public void SetOnGUICallback (Action<Rect> callback)
		{
			_callback = callback;
		}

		public GUISkin GetSkin ()
		{
			return _skin;
		}
        
        private readonly List<MenuItem> _items = new List<MenuItem>();
        private GUISkin _skin;
        
        private float _menuBarHeight = 25.0f;
        private float _menuItemWidth;
        private GUILayoutOption _menuItemWidthOption;
		private Action<Rect> _callback;
	}
}
