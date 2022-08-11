//
///********************************************************************
//created:    2014-02-19
//author:     lixianmin
//
//Copyright (C) - All Rights Reserved
//*********************************************************************/
//using System;
//using System.Collections.Generic;
//
//namespace Unicorn.AutoCode
//{
//    public class Kind
//    {
//		protected Kind ()
//		{
//
//		}
//
//		public static Kind Create (Type type, string[] importedNamespaces = null)
//		{
//			if (null != type)
//			{
//				Kind kind = null;
//				importedNamespaces = importedNamespaces ?? _defaultImportedNameSpaces;
//
//				if (type.IsGenericType)
//				{
//					var genericTypeDefinition = type.GetGenericTypeDefinition();
//					
//					if (typeof(List<>) == genericTypeDefinition)
//					{
//						kind = new GenericListKind();
//					}
//					else 
//					{
//						kind = new GenericKind();
//					}
//				}
//				else
//				{
//					kind = new Kind();
//				}
//
//				kind._type = type;
//				return kind;
//			}
//
//			return null;
//		}
//
//		public bool IsImported ()
//		{
//			if (_importedNamespaces.Length > 0)
//			{
//				var typeSpace = _type.Namespace;
//				
//				foreach (var space in _importedNamespaces)
//				{
//					if (space == typeSpace)
//					{
//						return true;
//					}
//				}
//			}
//			
//			return false;
//		}
//
//		public virtual string GetCodeName ()
//		{
//			if (_type == typeof(object))
//			{
//				return "object";
//			}
//			else if (_type == typeof(string))
//			{
//				return "string";
//			}
//
//			var typeName = IsImported() ? _type.Name : _type.FullName.Replace('+', '.');
//			return typeName;
//		}
//
//		public Type GetRawType ()
//		{
//			return _type;
//		}
//
//		protected Type 		_type;
//		protected string[]	_importedNamespaces;
//
//		private static readonly string[] _defaultImportedNameSpaces = new string[0];
//    }
//}