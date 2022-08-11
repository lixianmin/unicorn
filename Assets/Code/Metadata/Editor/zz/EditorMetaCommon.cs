
/********************************************************************
created:    2014-03-25
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using Unicorn;

namespace Metadata
{
    public static class EditorMetaCommon
    {
//		public static void Init (MetaManager metaManager)
//		{
//			if (null != metaManager)
//			{
//				_baseTypes.Clear();
//
//				foreach (var type in metaManager.MetaTypes)
//				{
//					var baseType = type.RawType.BaseType;
//					if (null != baseType)
//					{
//						_baseTypes.Add(baseType);
//					}
//				}
//			}
//		}
//
//		public static bool IsFinalType (Type type)
//		{
//			if (null != type)
//			{
//				var isFinal = !_baseTypes.Contains(type);
//				return isFinal;
//			}
//
//			return false;
//		}
//
//		public static string GetMetaTypeName (Type type)
//		{
//			var fullname = type.FullName;
//			
//			var withoutNamespace = fullname;
//			if (type.Namespace != null)
//			{
//				withoutNamespace = fullname.Substring(type.Namespace.Length + 1);
//			}
//			
//			return withoutNamespace.Replace('+', '_').Replace('.', '_');
//		}
//
//        public static bool IsAutoCodeIgnore (MemberInfo member)
//        {
//            var ignore = member.GetCustomAttributes(typeof(AutoCodeIgnoreAttribute), false).Length > 0;
//            return ignore;
//        }
//
//		internal static Type GetAttributeValueRawType (Type type)
//		{
//			if (null != type)
//			{
//                if (type == typeof(Int32_t))
//                {
//                    return typeof(int);
//                }
//                else if (type == typeof(Int64_t))
//                {
//                    return typeof(long);
//                }
//                else if (type == typeof(Float_t))
//                {
//                    return typeof(float);
//                }
//			}
//
//			return null;
//		}
//        
//        public static Type GetRootMetadata (Type type)
//        {
//            while(true)
//            {
//                var baseType = type.BaseType;
//
//				if(!MetaTools.IsMetadata(baseType))
//                {
//                    break;
//                }
//
//                type = baseType;
//            }
//
//            return type;
//        }

        public static string GetNestedClassName (Type type)
        {
            var fullname = type.FullName;
            var candidateFullName = fullname;

			var typeNamespace = type.Namespace;
			if (typeNamespace != null)
            {
				if (typeNamespace == "Metadata"
				    || typeNamespace == "System"
				    || typeNamespace == "UnityEngine"
				    )
				{
					candidateFullName = fullname.Substring(typeNamespace.Length + 1);
				}
            }

            var className = !type.IsNested ? candidateFullName : candidateFullName.Replace('+', '.');
			return className;
        }



//		internal static IEnumerable<PropertyInfo> GetExportableProperties (Type type, BindingFlags flags)
//		{
//			if (null != type)
//			{
//				foreach (var property in type.GetProperties(flags))
//				{
//					if (!property.CanRead || !property.CanWrite)
//					{
//						continue;
//					}
//
//					var propertyAttributes = property.GetCustomAttributes(typeof(ExportAttribute), false);
//					if (null == propertyAttributes)
//					{
//						continue;
//					}
//
//					var exportFlags = (propertyAttributes[0] as ExportAttribute).GetExportFlags();
//					var isExportable = (exportFlags & ExportFlags.Exportable) != 0;
//					if (!isExportable)
//					{
//						continue;
//					}
//
//					if (EditorMetaCommon.IsAutoCodeIgnore(property))
//					{
//						continue;
//					}
//
//					yield return property;
//				}
//			}	
//		}
//
//		public static CodeAssembly GetCodeAssembly (Type type)
//		{
//			var codeAssembly = CodeAssembly.None;
//			var fullname = type.Assembly.FullName;
//
//			if (fullname.StartsWithEx("Assembly-CSharp-firstpass", CompareOptions.Ordinal))
//			{
//				codeAssembly = CodeAssembly.StandardAssembly;
//			}
//			else if (fullname.StartsWithEx("Assembly-CSharp-Editor", CompareOptions.Ordinal))
//			{
//				codeAssembly = CodeAssembly.EditorAssembly;
//			}
//			else 
//			{
//				codeAssembly = CodeAssembly.ClientAssembly;
//			}
//
//			return codeAssembly;
//		}
//
//		private static string _standardAutoCodeDirectory;
//		public static string StandardAutoCodeDirectory
//		{
//			get
//			{
//				if (null == _standardAutoCodeDirectory)
//				{
//					_standardAutoCodeDirectory = Application.dataPath + "/Standard Assets/Code/Metadata/AutoCode/";
//				}
//				
//				return _standardAutoCodeDirectory;
//			}
//		}
//
//		private static string _clientAutoCodeDirectory;
//        public static string ClientAutoCodeDirectory
//        {
//            get
//            {
//				if (null == _clientAutoCodeDirectory)
//				{
//					_clientAutoCodeDirectory = Application.dataPath + "/Code/Metadata/AutoCode/";
//				}
//
//				return _clientAutoCodeDirectory;
//            }
//        }

//		private static string _editorAutoCodeDirectory;
//		public static string EditorAutoCodeDirectory
//		{
//			get
//			{
//				if (null == _editorAutoCodeDirectory)
//				{
//					_editorAutoCodeDirectory = Application.dataPath + "/Code/Metadata/Editor/AutoCode/";
//				}
//				
//				return _editorAutoCodeDirectory;
//			}
//		}
//        
//        public static string DrawableObjectsDirectory
//        {
//            get
//            {
//                var directory = Application.dataPath +"/Code/Metadata/Editor/DrawableObjects/";
//                return directory;
//            }
//        }
//
//		private static string _localeTextTranslatorPath = os.path.join(PathTools.EditorResourceRoot, "locale_text_translator.xml");
//		public static string LocaleTextTranslatorPath
//		{
//			get { return _localeTextTranslatorPath; }
//			set { _localeTextTranslatorPath = value; }
//		}
//		
//		private static string _metadataRoot;

//        public const string MenuRoot 		= "*Metadata/";
//		public const string NamespaceName	= "Metadata";

//		private static HashSet<Type> _baseTypes = new HashSet<Type>();
    }
}