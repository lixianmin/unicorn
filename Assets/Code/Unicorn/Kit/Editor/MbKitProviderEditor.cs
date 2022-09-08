
/********************************************************************
created:    2022-09-06
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unicorn
{
    [CustomEditor(typeof(MbKitProvider), true)]
    [CanEditMultipleObjects]
    public class MbKitProviderEditor : Editor
    {
        protected void OnEnable ()
        {
            _target = serializedObject.targetObject as MbKitProvider;
            _assetsProperty = serializedObject.FindProperty("assets");
        }

        public override void OnInspectorGUI ()
        {
            serializedObject.Update();
            {
                _target.fullKitName = EditorGUILayout.TextField(_fullKitNameLabel, _target.fullKitName);
                
                // 当_kitNameHits是null的时间, 需要初始化一次
                if (_target.fullKitName != _lastFullKitName || _kitNameHits == null)
                {
                    _lastFullKitName = _target.fullKitName;
                    _idxSelection = -1; // 重置选中的索引
                    
                    _ResetFullKitNameLabel();
                    
                    // 收集hints需要的kit name
                    _kitNameHitsResults.Clear();
                    KitFactory.Search(_target.fullKitName, _kitNameHitsResults);
                    _kitNameHits = _kitNameHitsResults.Take(100).ToArray();
                }
                
                // hits部分
                _hitScrollPosition = GUILayout.BeginScrollView(_hitScrollPosition, GUILayout.Height(100));
                {
                    if (_kitNameHits.Length > 0)
                    {
                        var lastIndex = _idxSelection;
                        _idxSelection = GUILayout.SelectionGrid(_idxSelection, _kitNameHits, 1);
                        if (_idxSelection != lastIndex)
                        {
                            _lastFullKitName = _kitNameHits[_idxSelection];
                            _target.fullKitName = _lastFullKitName;
                            
                            _ResetFullKitNameLabel();
                            
                            // EditorUtility.SetDirty(_target);
                            // 如果当前是TextField拥有输入焦点, 则设置fullKitName后TextField的文本是不会变化的, 所以需要把输入焦点切走
                            GUI.FocusControl(string.Empty); 
                        }
                    }
                }
                GUILayout.EndScrollView();

                // _assets
                EditorGUILayout.PropertyField(_assetsProperty, true);    
            }
            
           
            serializedObject.ApplyModifiedProperties();
        }

        private void _ResetFullKitNameLabel()
        {
            _fullKitNameLabel = "Full Kit Name";

            // 如果是有效的kit name, 则把sort也打印出来
            var kit = KitFactory.Create(_lastFullKitName);
            if (kit is not null)
            {
                _fullKitNameLabel = $"Full Kit Name [sort={TypeTools.SetDefaultTypeIndex(kit.GetType())}]";
            }
        }
        
        private string _lastFullKitName = "invalid default text";
        private string _fullKitNameLabel;
        private readonly List<string> _kitNameHitsResults = new();
        private string[] _kitNameHits;
        private Vector2 _hitScrollPosition;
        private int _idxSelection = -1;

        private SerializedProperty _assetsProperty;
        private MbKitProvider _target;
    }
}