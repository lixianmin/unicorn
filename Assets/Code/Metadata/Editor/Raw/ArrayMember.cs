//
///********************************************************************
//created:    2014-08-27
//author:     lixianmin
//
//Copyright (C) - All Rights Reserved
//*********************************************************************/
//using UnityEngine;
//using System;
//using System.IO;
//using Unicorn;
//using Unicorn.AutoCode;
//
//namespace Metadata.Raw
//{
//    class ArrayMember: MemberBase
//    {
//        public override void WriteLoad (CodeWriter writer)
//        {
//			string childtype = EditorMetaCommon.GetNestedClassName(_type.GetElementType());
//            string prefix = childtype;
//            string postfix = string.Empty;
//            int first = childtype.IndexOf("[", StringComparison.Ordinal);
//            if (first >= 0)
//            {
//                prefix = childtype.Substring(0, first);
//                postfix = childtype.Substring(first);
//            }
//
//			writer.WriteLine("{0} = new {1}[reader.ReadInt32()]{2};", _name, prefix, postfix);
//            writer.WriteLine("for (int i{1} = 0; i{1} < {0}.Length; i{1}++)", _name, writer.Indent);
//            using (CodeScope.CreateCSharpScope(writer))
//            {
//                _WriteLoadType(writer, _type.GetElementType(), _name + string.Format("[i{0}]", writer.Indent - 1));
//            }
//        }
//        
//        public override void WriteSave (CodeWriter writer)
//        {
//            writer.WriteLine("int {0}Length = null != {0} ? {0}.Length : 0 ;", _name);
//            writer.WriteLine("writer.Write ({0}Length);", _name);
//            writer.WriteLine("for (int i{1}= 0; i{1} < {0}Length; ++i{1})", _name, writer.Indent);
//
//            using (CodeScope.CreateCSharpScope(writer))
//            {
//                writer.WriteLine("var item{1} = {0}[i{1}];", _name, writer.Indent - 1);
//                var elementType = _type.GetElementType();
//                _WriteSaveType(writer, elementType, string.Format("item{0}", writer.Indent - 1));
//            }
//        }
//
//		public override void WriteNotEqualsReturn (CodeWriter writer)
//		{
//			writer.WriteLine("int {0}Length1 = null != {0} ? {0}.Length : 0 ;", _name);
//			writer.WriteLine("int {0}Length2 = null != that.{0} ? that.{0}.Length : 0 ;", _name);
//			writer.WriteLine("if ({0}Length1 != {0}Length2)", _name);
//			using (CodeScope.CreateCSharpScope(writer))
//			{
//				writer.WriteLine("return false;");
//			}
//
//			writer.WriteLine("for (int i{1}= 0; i{1} < {0}Length1; ++i{1})", _name, writer.Indent);
//			
//			using (CodeScope.CreateCSharpScope(writer))
//			{
//				var elementType = _type.GetElementType();
//				var itemName = string.Format("{0}[i{1}]", _name, writer.Indent - 1);
//				_WriteNotEqualsReturn(writer, elementType, itemName);
//			}
//		}
//    }
//}