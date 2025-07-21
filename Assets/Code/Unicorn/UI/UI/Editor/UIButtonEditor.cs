
/********************************************************************
created:    2017-08-30
author:     lixianmin

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*********************************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

namespace Unicorn.UI
{
    [CustomEditor(typeof(UIButton), true)]
    [CanEditMultipleObjects]
    public class ButtonEditor : SelectableEditor
    {
        protected override void OnEnable ()
        {
            base.OnEnable();

            // _target = serializedObject.targetObject as UIButton;

            _onClickProperty = serializedObject.FindProperty("m_OnClick");
            // _onLongClickProperty = serializedObject.FindProperty("onLongClick");
        }

        public override void OnInspectorGUI ()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(_onClickProperty);

            // GUILayout.BeginHorizontal();
            // {
            //     _target.hasOnLongClick = EditorGUILayout.Toggle(new GUIContent("OnLongClick", "Event called after a long click"), _target.hasOnLongClick);
            // }
            // GUILayout.EndHorizontal();

            // if (_target.hasOnLongClick)
            // {
            //     _target.onLongClick.holdTime = EditorGUILayout.FloatField("hold Time", _target.onLongClick.holdTime);
            //     EditorGUILayout.PropertyField(_onLongClickProperty);
            // }

            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }

        private SerializedProperty _onClickProperty;
        // private SerializedProperty _onLongClickProperty;

        // private UIButton _target;
    }
}
