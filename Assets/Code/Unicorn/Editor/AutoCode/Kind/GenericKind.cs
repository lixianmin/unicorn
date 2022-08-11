//
///********************************************************************
//created:    2014-02-19
//author:     lixianmin
//
//Copyright (C) - All Rights Reserved
//*********************************************************************/
//using System;
//using System.IO;
//
//namespace Unicorn.AutoCode
//{
//    public class GenericKind: Kind
//    {
//		public override string GetCodeName ()
//		{
//			var typeName = string.Concat(base.GetCodeName(), "<", GetGenericArgumentsText(), ">");
//			return typeName;
//		}
//
//		public string GetGenericArgumentsText ()
//		{
//			var text = ", ".JoinEx(_type.GetGenericArguments()
//			                       , argType => Kind.Create(argType, _importedNamespaces).GetCodeName());
//			return text;
//		}
//    }
//}