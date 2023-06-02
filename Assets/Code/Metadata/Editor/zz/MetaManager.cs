//
///********************************************************************
//created:    2014-02-19
//author:     lixianmin
//
//Copyright (C) - All Rights Reserved
//*********************************************************************/
//using System;
//using System.Reflection;
//using System.Collections.Generic;
//using System.Linq;
//using Unicorn;
//using Unicorn.Collections;
//
//namespace Metadata
//{
//    public class MetaManager
//    {
//        public MetaManager ()
//        {
//            foreach (var type in MetaTypeTools.GetMetaTypes())
//            {
//                if (type.IsTemplate)
//                {
//                    _templateTypes.Add(type);
//                } 
//                else if (type.IsConfig)
//                {
//                    _configTypes.Add(type);
//                }
//            }
//        }
////		private SortedTable<string, int> _GetLastIndexTable ()
////		{
////			var lastIndexTable = new SortedTable<string, int>();
////			var enumType = TypeTools.SearchType("Metadata.MetadataType");
////
////			if (null != enumType)
////			{
////				foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
////				{
////					var key = field.Name;
////					var val = int.Parse(field.GetRawConstantValue().ToString());
////					
////					lastIndexTable.Add(key, val);
////				}
////			}
////
////			return lastIndexTable;
////		}
////
////		private void _InitMetaIndices ()
////		{
////			var rawIndex = MetadataManager.RawMetadataBaseIndex;
////			var luaIndex = 0;
////			var lastIndexTable = _GetLastIndexTable();
////
////			foreach (var type in MetaTypes)
////			{
////				var lastMetaIndex = lastIndexTable.GetEx(type.GetMetaTypeName(), -1);
////				if (lastMetaIndex > 0)
////				{
////					type.MetaIndex = lastMetaIndex;
////
////					if (lastMetaIndex >= MetadataManager.RawMetadataBaseIndex)
////					{
////						rawIndex = Math.Max(lastMetaIndex, rawIndex);
////					}
////					else 
////					{
////                        luaIndex = Math.Max(lastMetaIndex, luaIndex);
////					}
////				}
////			}
////
////			foreach (var type in MetaTypes)
////			{
////				if (type.IsAbstract || type.MetaIndex > 0)
////				{
////					continue;
////				}
////
////				var codeAssembly = type.GetCodeAssembly();
////				if (codeAssembly != CodeAssembly.EditorAssembly)
////				{
////					type.MetaIndex = ++rawIndex;
////				}
////				else
////				{
////					type.MetaIndex = ++luaIndex;
////				}
////			}
////
////            _CheckMetaIndexDuplication(MetaTypes);
////		}
////
////        private static void _CheckMetaIndexDuplication (MetaType[] metaTypes)
////        {
////            var history = new Dictionary<int, MetaType>(metaTypes.Length);
////            foreach (var type in metaTypes)
////            {
////                if (type.IsAbstract)
////                {
////                    continue;
////                }
////
////                var last = history.GetEx(type.MetaIndex);
////                if (null == last)
////                {
////                    history.Add(type.MetaIndex, type);
////                }
////                else
////                {
////                    Logo.Error("Indices duplication, rectify manually or regenerate all indices: index={0}, last={1}, current={2}"
////                        , type.MetaIndex.ToString(), last.RawType.FullName, type.RawType.FullName);
////                }
////            }
////        }
////
////		public IEnumerable<MetaType> EnumerateMetaTypes (bool editorAssembly)
////		{
////			if (editorAssembly)
////			{
////				foreach (var childType in _metaTypes)
////				{
////					if (childType.IsAbstract)
////					{
////						continue;
////					}
////					
////					var isEditorAssembly = childType.GetCodeAssembly() == CodeAssembly.EditorAssembly;
////					if (isEditorAssembly)
////					{
////						yield return childType;
////					}
////				}
////			}
////			else 
////			{
////				foreach (var childType in _metaTypes)
////				{
////					if (childType.IsAbstract)
////					{
////						continue;
////					}
////					
////					var isEditorAssembly = childType.GetCodeAssembly() == CodeAssembly.EditorAssembly;
////					if (!isEditorAssembly)
////					{
////						yield return childType;
////					}
////				}
////			}
////		}
////		
////        public IEnumerable<MetaType> EnumerateGameMetaTypes ()
////        {
////            foreach (var childType in MetaTypeTools.GetMetaTypes())
////            {
////                if (childType.IsAbstract)
////                {
////                    continue;
////                }
////
////                var isEditorAssembly = childType.GetCodeAssembly() == CodeAssembly.EditorAssembly;
////                if (!isEditorAssembly)
////                {
////                    yield return childType;
////                }
////            }
////        }
////
////		public IEnumerable<MetaType> TemplateTypes	{ get { return _templateTypes;  } }
////        public IEnumerable<MetaType> ConfigTypes	{ get { return _configTypes; } }
//        public MetaType[]			 MetaTypes		{ get { return MetaTypeTools.GetMetaTypes(); } }
//
//        private readonly List<MetaType> _configTypes = new List<MetaType>();
//        private readonly List<MetaType> _templateTypes = new List<MetaType>();
//    }
//}