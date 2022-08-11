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
//    public class GenericListKind: GenericKind
//    {
//		public override string GetCodeName ()
//		{
//			var argumentType = _type.GetGenericArguments()[0];
//			var argumentTypeName = Kind.Create(argumentType, _importedNamespaces).GetCodeName();
//
//			var typeName = string.Concat(base.GetCodeName(), "<", argumentTypeName, ">");
//			return typeName;
//		}
//    }
//}