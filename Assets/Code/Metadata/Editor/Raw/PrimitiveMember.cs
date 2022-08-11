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
//    class PrimitiveMember: MemberBase
//    {
//        public override void WriteLoad (CodeWriter writer)
//        {
//			writer.WriteLine("{0} = reader.Read{1}();", _name, _type.Name);
//        }
//        
//        public override void WriteSave (CodeWriter writer)
//        {
//            writer.WriteLine("writer.Write({0});", _name);
//        }
//
//		public override void WriteNotEqualsReturn (CodeWriter writer)
//		{
//			writer.WriteLine("if ({0} != that.{0})", _name);
//			using (CodeScope.CreateCSharpScope(writer))
//			{
//				writer.WriteLine("return false;");
//			}
//		}
//    }
//}