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
//    class ListMember: MemberBase
//    {
//        public override void WriteLoad (CodeWriter writer)
//        {
//            Type elementType = _type.GetGenericArguments() [0];
//            string elementTypeName = EditorMetaCommon.GetNestedClassName(elementType);
//            
//			writer.WriteLine ("int {0}Count = reader.ReadInt32();", _name);
//			writer.WriteLine ("if (null == {0}) {{ {0} = new List<{1}>({0}Count); }} else {{ {0}.Clear(); }}", _name, elementTypeName);
//            writer.WriteLine ("for (int index= 0; index < {0}Count; ++index)", _name);
//
//            using (CodeScope.CreateCSharpScope(writer))
//            {
//                writer.WriteLine ("{0}.Add (default({1}));", _name, elementTypeName);
//                _WriteLoadType (writer, elementType, _name + "[index]");
//            }
//        }
//        
//        public override void WriteSave (CodeWriter writer)
//        {
//            writer.WriteLine ("int {0}Count = null != {0} ? {0}.Count : 0 ;", _name);
//            writer.WriteLine ("writer.Write ({0}Count);", _name);
//            writer.WriteLine ("for (int index= 0; index < {0}Count; ++index)", _name);
//
//            using (CodeScope.CreateCSharpScope (writer))
//            {
//                writer.WriteLine ("var item = {0}[index];", _name);
//                var elementType = _type.GetGenericArguments() [0];
//                _WriteSaveType (writer, elementType, "item");
//            }
//        }
//
//		public override void WriteNotEqualsReturn (CodeWriter writer)
//		{
//			writer.WriteLine ("int {0}Count1 = null != {0} ? {0}.Count : 0 ;", _name);
//			writer.WriteLine ("int {0}Count2 = null != that.{0} ? that.{0}.Count : 0 ;", _name);
//			writer.WriteLine("if ({0}Count1 != {0}Count2)", _name);
//			using (CodeScope.CreateCSharpScope(writer))
//			{
//				writer.WriteLine("return false;");
//			}
//
//			writer.WriteLine("for (int i{1}= 0; i{1} < {0}Count1; ++i{1})", _name, writer.Indent);
//			
//			using (CodeScope.CreateCSharpScope(writer))
//			{
//				var elementType = _type.GetGenericArguments() [0];
//				var itemName = string.Format("{0}[i{1}]", _name, writer.Indent - 1);
//				_WriteNotEqualsReturn(writer, elementType, itemName);
//			}
//		}
//    }
//}