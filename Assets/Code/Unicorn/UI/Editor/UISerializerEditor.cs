
/********************************************************************
created:    2017-11-27
author:     lixianmin

*********************************************************************/

using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace Unicorn.UI
{
    internal static class UISerializerEditor
    {
        private static UISerializer.WidgetData _FillWidgetData (Transform root, string key, string name, string type)
        {
            var trans = _DeepFindWidget (root, name);
            if (null == trans)
            {
                return null;
            }

            var item = new UISerializer.WidgetData
            {
                name = name,
                type = type
            };

            var target = _GetWidgetTarget (trans, type);
            item.target = target;
            if (null != target)
            {
                if (target is UIText label)
                {
                    if (!string.IsNullOrEmpty(label.GetGUID()))
                    {
                        item.userdata = label.GetGUID();
                    }
                }
            }
            else
            {
                Console.Error.WriteLine("[_FillWidgetData()] Can not find a component with name={0}, type={1}", name, type);
            }

            return item;
        }

        private static Component _GetWidgetTarget (Transform trans, string type)
        {
            var target = trans.GetComponent(type);
            if (null == target && type.StartsWith("UI."))
            {
                var script = trans.GetComponent(typeof(UISerializer));
                if (null != script)
                {
                    target = script;
                }
            }

            return target;
        }

        private static Transform _DeepFindWidget (Transform father, string name)
        {
            if (null != father && null != name)
            {
                foreach (var node in _ForEachNode(father))
                {
                    if (node.name == name)
                    {
                        return node;
                    }
                }
            }

            return null;
        }

        private static UISerializer.WidgetData _GetWidgetData (IList<UISerializer.WidgetData> dataList, string name, string type)
        {
            if (null != dataList)
            {
                var count = dataList.Count;
                for (int i = 0; i < count; i++)
                {
                    var item = dataList[i];
                    if (item.name == name && item.type == type)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        private static void _SavePrefab (Transform root)
        {
            EditorUtility.SetDirty(root.gameObject);
            AssetDatabase.SaveAssets();
        }

        private static List<Type> _FetchAllWindowTypes()
        {
            if (_allWindowTypes == null)
            {
                _allWindowTypes = new();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    if (assembly.IsDynamic)
                    {
                        continue;
                    }
                
                    foreach (var type in assembly.GetExportedTypes())
                    {
                        if (type.IsSubclassOf(typeof(UIWindowBase)))
                        {
                            _allWindowTypes.Add(type);
                        }
                    }
                }
            }

            return _allWindowTypes;
        }
        
        private static UIWindowBase _SearchWindow (string prefabName)
        {
            var windowTypes = _FetchAllWindowTypes();
            var count = windowTypes.Count;
            for (var i = 0; i < count; i++)
            {
                var windowType = windowTypes[i];
                var window = Activator.CreateInstance(windowType) as UIWindowBase;
                var resourcePath = window?.GetResourcePath();
                if (resourcePath?.LastIndexOf(prefabName) > 0)
                {
                    return window;
                }
            }

            return null;
        }

        private static void _CollectWidgetFromCode (Transform root, IList<UISerializer.WidgetData> dataList)
        {
            var window = _SearchWindow(root.name);
            var layouts = _GetLayouts(window);
            if (layouts.Count == 0)
            {
                return;
            }
            
            foreach (var layout in layouts)
            {
                var name = layout.name;
                var type = layout.type.Name;
                var lastData = _GetWidgetData(dataList, name, type);
                if (null != lastData)
                {
                    Console.Error.WriteLine("[_CollectWidgetFromCode()] Found an old widgetData with the same name={0}, type={1}", name, type);
                    continue;
                }

                var currentData = _FillWidgetData (root, string.Empty, name, type);
                if (null == currentData)
                {
                    Console.Error.WriteLine("[_CollectWidgetFromCode()] Can not find a widgetData with name = {0} ", name);
                    return;
                }

                _AddUniqueWidgetData(dataList, currentData);
            }
        }

        private static IList<Layout> _GetLayouts(UIWindowBase window)
        {
            var list = new List<Layout>();
            if (window == null)
            {
                return list;
            }

            var type = window.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                var fieldType = field.FieldType;
                if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(UIWidget<>))
                {
                    var argType1 = fieldType.GetGenericArguments()[0];
                    var widget = field.GetValue(window) as UIWidgetBase;
                    list.Add(new Layout {name = widget?.GetName(), type = argType1});
                }
            }
            
            return list;
        }
        
        private static void _AddUniqueWidgetData (IList<UISerializer.WidgetData> dataList, UISerializer.WidgetData current)
        {
            if (null == dataList)
            {
                return;
            }
            
            var count = dataList.Count;
            for (var i = 0; i < count; i++)
            {
                var item = dataList[i];
                if (item.target == current.target)
                {
                    return;
                }
            }

            dataList.Add(current);
        }

        private static void _FetchLabels (UISerializer script)
        {
            var labelList = ListPool<Text>.Spawn();

            foreach (var node in _ForEachNode(script.transform))
            {
                var text = node.GetComponent<Text>();
                if (null != text)
                {
                    labelList.Add(text);
                }
            }

            script.labels = labelList.ToArray();
            ListPool<Text>.Recycle(labelList);
        }

//        [MenuItem("Assets/Test", false, 0)]
//        private static void _BroadcastControlMenu ()
//        {
//            var goSelected = Selection.activeObject as GameObject;
//            if (null == goSelected)
//            {
//                Console.Warning.WriteLine("Please select a prefab.");
//                return;
//            }
//
//            var sbText = new System.Text.StringBuilder();
//            foreach (var trans in _ForEachNode(goSelected.transform))
//            {
//                sbText.AppendLine(trans.name);
//            }
//
//            Console.WriteLine(sbText);
//        }

        private static IEnumerable<Transform> _ForEachNode (Transform father)
        {
            if (null == father) yield break;
            yield return father;

            var childCount = father.childCount;
            for (var i= 0; i< childCount; ++i)
            {
                var child = father.GetChild(i);
                var isNotControl = null == child.GetComponent(typeof(UISerializer));
                if (isNotControl)
                {
                    foreach (var node in _ForEachNode(child))
                    {
                        yield return node;
                    }
                }
                else
                {
                    yield return child;
                }
            }
        }

        private static void _CollectUITextWithGUID (UISerializer script, List<UISerializer.WidgetData> dataList)
        {
            var transform = script.transform;

            foreach (var item in script.labels)
            {
                var label = item as UIText1;
                if (null == label || string.IsNullOrEmpty(label.GetGUID()))
                {
                    continue;
                }

                var name = label.name;
                var type = "UIText";
                var lastData = _GetWidgetData(dataList, name, type);
                if (null != lastData)
                {
                    continue;
                }

                var currentData = _FillWidgetData(transform, name, name, type);
                if (null != currentData)
                {
                    _AddUniqueWidgetData(dataList, currentData);
                }
            }
        }

        private static void _CheckNameDuplication (Transform root)
        {
            var traversedTable = new Dictionary<string, Transform>();

            foreach (var child in _ForEachNode(root))
            {
                var name = child.name;
                if (name.Contains("(") || name.Contains(")"))
                {
                    var currentPath = child.GetFindPathEx();
                    Console.Error.WriteLine("\"()\" is not allowed in gameObject names, path={0}/{1}", root.name, currentPath);
                }

                var transLast = traversedTable.GetEx(name);
                if (null != transLast)
                {
                    string lastPath = transLast.GetFindPathEx();
                    string currentPath = child.GetFindPathEx();
                    Console.Error.WriteLine("Duplication name found: lastPath={0}/{1}\n, currentPath={0}/{2}", root.name, lastPath, currentPath);
                }
                else
                {
                    traversedTable.Add(name, child);
                }
            }
        }

        private static void _CheckEventSystemExistence (Transform transform)
        {
            var script = transform.GetComponentInChildren<UnityEngine.EventSystems.EventSystem>(true);
            if (null != script)
            {
                var path = script.transform.GetFindPathEx();
                Console.Warning.WriteLine("Caution: detect an EventSystem script ({0})", path);
            }
        }

        public static void SerializePrefab (UISerializer rootScript)
        {
            Console.WriteLine ("Begin serializing **********************");

            var root = rootScript.transform;

            _CheckNameDuplication(root);
            _CheckEventSystemExistence(root);

            rootScript.widgetDatas = null;
            var dataList = ListPool<UISerializer.WidgetData>.Spawn();
            _CollectWidgetFromCode(root, dataList);

            _FetchLabels(rootScript);
            _CollectUITextWithGUID(rootScript, dataList);

            rootScript.widgetDatas = dataList.ToArray();
            ListPool<UISerializer.WidgetData>.Recycle(dataList);

            _SavePrefab(root);

            Console.WriteLine ("End serializing **********************");
        }
        
        [MenuItem("Assets/*Serialize Prefab", true)]
        private static bool _SerializePrefab_Validate()
        {
            var prefab = Selection.activeGameObject;
            if (prefab is null)
            {
                return false;
            }
            
            var script = prefab.GetComponent<UISerializer>();
            if (script is null)
            {
                prefab.AddComponent<UISerializer>();
            }
            
            return true;
        }
        
        [MenuItem("Assets/*Serialize Prefab", false, 2000)]
        private static void _SerializePrefab()
        {
            var prefab = Selection.activeGameObject;
            var script = prefab.GetComponent<UISerializer>();
            SerializePrefab(script);
        }
        
        private static List<Type> _allWindowTypes;
    }
}