
/********************************************************************
created:    2014-01-13
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Unicorn;

namespace Metadata.Menus
{
    public class PrototypeEditor: EditorWindow
    {
        private enum MetadataType {
            Template = 0,
            Config = 1
        }
        
        [MenuItem(EditorMetaTools.MenuRoot + "Prototype Editor", false, 109)]
        public static void OpenEditor ()
        {
            var editor = EditorWindow.GetWindow<PrototypeEditor>(false, "Prototype Generator", true);
            editor.Show();
        }

        private void OnFocus ()
        {
            var prefix = "Metadata.";
            var startIndex = prefix.Length;

            _templateNames = (from type in EditorMetaTools.EnumerateTemplateTypes() where !type.IsAbstract select type.RawType.ToString().Substring(startIndex)).ToArray();
            _configNames = (from type in EditorMetaTools.EnumerateConfigTypes() where !type.IsAbstract select type.RawType.ToString().Substring(startIndex)).ToArray();

            if (null == _metadata)
            {
                var fullname = prefix + _GetMetadataName();
                var type = TypeTools.SearchType(fullname);
                if (null != type)
                {
                    _metadata = (IMetadata) Activator.CreateInstance(type);
                }
            }
        }

        private string _GetMetadataName ()
        {
            var metadataName = string.Empty;
            if(_metadataType == MetadataType.Template)
            {
                var index = _idxTemplateType;

                if(index >= 0 && index < _templateNames.Length)
                {
                    metadataName = _templateNames[index];
                }
            }
            else 
            {
                var index = _idxConfigType;

                if(index >= 0 && index < _configNames.Length)
                {
                    metadataName = _configNames[index];
                }
            }

            return metadataName;
        }

        private void OnGUI ()
        {
            var metadataType = (MetadataType) EditorGUILayout.EnumPopup("Metadata Type", _metadataType);
            if (_metadataType != metadataType)
            {
                _metadataType = metadataType;
                _CheckChangeMetadataType();
            }

            EditorGUILayout.Space();
            _DrawSearchField();

            EditorGUILayout.Space();
            if (null != _metadata)
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                {
                    InspectorTools.DrawObject(_metadata, _GetMetadataName());
                }
                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Generate"))
            {
                _GeneratePrototype();
            }
        }

        private void _DrawSearchField ()
        {
            _searchText = EditorGUILayout.TextField("Search: ", _searchText);
            if (_metadataType == MetadataType.Template)
            {
                if (_searchText != _lastSearchText) 
                {
                    for (int i = 0; i < _templateNames.Length; ++i) 
                    {
                        if (_templateNames[i].ToLower().Contains(_searchText.ToLower())) 
                        {
                            _idxTemplateType = i;
                            _lastSearchText = _searchText;
                            _CheckChangeMetadataType();
                            break;
                        }
                    }
                }
                var idxTemplateType = EditorGUILayout.Popup("Template types: ", _idxTemplateType, _templateNames);
                if (_idxTemplateType != idxTemplateType)
                {
                    _idxTemplateType = idxTemplateType;
                    _CheckChangeMetadataType();
                }
            }
            else
            {
                if (_searchText != _lastSearchText) 
                {
                    for (int i = 0; i < _configNames.Length; ++i) 
                    {
                        if (_configNames[i].ToLower().Contains(_searchText.ToLower())) 
                        {
                            _idxConfigType = i;
                            _lastSearchText = _searchText;
                            _CheckChangeMetadataType();
                            break;
                        }
                    }
                }

                var idxConfigType = EditorGUILayout.Popup("Config types: ", _idxConfigType, _configNames);
                if (_idxConfigType != idxConfigType)
                {
                    _idxConfigType = idxConfigType;
                    _CheckChangeMetadataType();
                }
            }
        }

        private void _CheckChangeMetadataType ()
        {
            var fullname = "Metadata." + _GetMetadataName();
            var type = TypeTools.SearchType(fullname);
            _metadata = null;

            if (null != type)
            {
                _metadata = (IMetadata) Activator.CreateInstance(type);
                FillMetadata(_metadata);
            }
        }

        /// <summary>
        /// Fill some field of the metadata.
        /// </summary>
        /// <param name="metadata">Metadata.</param>
        public static void FillMetadata (object metadata)
        {
            if (null == metadata)
			{
				return;
			}

            var fields = metadata.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach(var field in fields)
            {
                var fieldType = field.FieldType;

                if(fieldType.IsArray)
                {
                    const int defaultArrayLength = 2;
                    var elementArray = (Array) field.GetValue(metadata);
                    int count;
                    if(null == elementArray)
                    {
                        elementArray = (Array) Activator.CreateInstance(fieldType, defaultArrayLength);
                        field.SetValue(metadata, elementArray);
                        count = defaultArrayLength;
                    }
                    else
                    {
                        count = elementArray.Length;
                    }
                    
                    var elementType = fieldType.GetElementType();
                    for(int i= 0; i< count; ++i)
                    {
                        object elementObj = _GetTypeValue(elementType);
                        elementArray.SetValue(elementObj, i);
                        FillMetadata(elementObj);
                    }
                }
                else 
                {
                    var fieldValue= _GetTypeValue(fieldType);
                    field.SetValueEx(metadata, fieldValue);
                }
            }
        }

        private static object _GetTypeValue (Type type)
        {
            if (type == typeof (LocaleText))
            {
                return new LocaleText{ text = type.Name };
            }
            else if (type == typeof (VInt3))
            {
                return new VInt3();
            }
            else if (type == typeof (Int32_t))
            {
                Int32_t item = 0;
                return item;
            }
            else if (type == typeof (Int64_t))
            {
                Int64_t item = 0;
                return item;
            }
            else if (type == typeof (Float_t))
            {
                Float_t item = 0;
                return item;
            }
            else if (type == typeof(string))
            {
                return type.Name;
            }
            else if (MetaTools.IsMetadata(type))
            {
                var obj = Activator.CreateInstance(type);
                FillMetadata(obj);
                return obj;
            }

            return default(Type);
        }
        
        private void _GeneratePrototype ()
        {
            var xml = new XmlMetadata();

            if (_metadataType == MetadataType.Template)
            {
                var template = _metadata as Template;
                xml.Templates.Add(template);
            }
            else
            {
                var config = _metadata as Config;
                xml.Configs.Add(config);
            }

            var metadataName = _metadata.GetType().Name;
            var filepath = GetDesktopFilePath(metadataName);

            using (var writer = new StreamWriter(filepath))
            {
                writer.NewLine = os.linesep;
                var serializer = new XmlMetadataSerializer();
                serializer.Serialize(writer, xml);
            }
            
            Kernel.StartFile(filepath, null, true);
        }

        private static string GetDesktopFilePath (string name)
        {
            name = name ?? string.Empty;
            var filepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + name + ".xml";
            return filepath;
        }

        private MetadataType _metadataType;
        private int _idxTemplateType;
        private int _idxConfigType;
        private string[] _templateNames;
        private string[] _configNames;

        private IMetadata _metadata;
		private string _searchText;
		private string _lastSearchText;
        
        private Vector2 _scrollPosition;
    }
}