
/********************************************************************
created:    2022-09-06
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using UnityEditor;
using UnityEditor.UI;

namespace Unicorn.Kit
{
    [CustomEditor(typeof(MBKitProvider), false)]
    [CanEditMultipleObjects]
    public class MBKitProviderEditor : Editor
    {
        protected  void OnEnable ()
        {
            _target = serializedObject.targetObject as MBKitProvider;
            _assetsProperty = serializedObject.FindProperty("assets");
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

                    var kit = MBKitProvider.CreateKit(_lastFullKitName);
                    if (kit is not null)
                    {
                        _fullKitNameLabel = $"Full Kit Name [sort={TypeTools.SetDefaultTypeIndex(kit.GetType())}]";
                    }
                }
                
                EditorGUILayout.PropertyField(_assetsProperty, true);    
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private SerializedProperty _assetsProperty;
        private string _lastFullKitName = "invalid default text";
        private string _fullKitNameLabel;

        private MBKitProvider _target;
    }
}