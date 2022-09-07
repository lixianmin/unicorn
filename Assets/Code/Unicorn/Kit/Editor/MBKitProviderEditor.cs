
/********************************************************************
created:    2022-09-06
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unicorn
{
    [CustomEditor(typeof(MBKitProvider), true)]
    [CanEditMultipleObjects]
    public class MBKitProviderEditor : Editor
    {
        protected void OnEnable ()
        {
            _target = serializedObject.targetObject as MBKitProvider;
            _assetsProperty = serializedObject.FindProperty("assets");
            
            // 初始化 _kitNameHits
            KitFactory.Search(_target!.fullKitName, _kitNameHints);
        }

        public override void OnInspectorGUI ()
        {
            serializedObject.Update();
            {
                _target.fullKitName = EditorGUILayout.TextField(_fullKitNameLabel, _target.fullKitName);
                if (_target.fullKitName != _lastFullKitName)
                {
                    _lastFullKitName = _target.fullKitName;
                    _fullKitNameLabel = "Full Kit Name";

                    // 如果是有效的kit name, 则把sort也打印出来
                    var kit = KitFactory.Create(_lastFullKitName);
                    if (kit is not null)
                    {
                        _fullKitNameLabel = $"Full Kit Name [sort={TypeTools.SetDefaultTypeIndex(kit.GetType())}]";
                    }
                    
                    // 收集hints需要的kit name
                    _kitNameHints.Clear();
                    KitFactory.Search(_target.fullKitName, _kitNameHints);
                }
                
                // hits部分
                _hitScrollPosition = GUILayout.BeginScrollView(_hitScrollPosition, GUILayout.Height(100));
                {
                    var count = Math.Min(_kitNameHints.Count, 100);
                    for (var i = 0; i < count; i++)
                    {
                        var fullKitName = _kitNameHints[i];
                        if (GUILayout.Button(fullKitName))
                        {
                            _target.fullKitName = fullKitName;
                            
                            // 如果当前是TextField拥有输入焦点, 则设置fullKitName后TextField的文本是不会变化的, 所以需要把输入焦点切走
                            GUI.FocusControl(fullKitName); 
                        }
                    }
                }
                GUILayout.EndScrollView();

                // _assets
                EditorGUILayout.PropertyField(_assetsProperty, true);    
            }
            
           
            serializedObject.ApplyModifiedProperties();
        }

        private string _lastFullKitName = "invalid default text";
        private string _fullKitNameLabel;
        private readonly List<string> _kitNameHints = new();
        private Vector2 _hitScrollPosition;

        private SerializedProperty _assetsProperty;
        private MBKitProvider _target;
    }
}