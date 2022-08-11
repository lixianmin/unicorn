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
//    class MetadataMember: MemberBase
//    {
//        public override void WriteLoad (CodeWriter writer)
//        {
//            if (_type.IsClass)
//            {
//				writer.WriteLine();
//				writer.WriteLine("if (reader.ReadBoolean())");
//                using (CodeScope.CreateCSharpScope(writer))
//                {
//                    _WriteLoadMetadata(writer);
//                }
//            }
//            else if (_type.IsValueType)
//            {
//				writer.WriteLine("{0}.Load(reader);", _name);
//			}
//        }
//        
//        public override void WriteSave (CodeWriter writer)
//        {
//            if (_type.IsClass)
//            {
//				writer.WriteLine();
//                writer.WriteLine("writer.Write(null != {0});", _name);
//                writer.WriteLine("if (null != {0})", _name);
//                using (CodeScope.CreateCSharpScope(writer))
//                {
//                    _WriteSaveMetadata(writer);
//                }
//            }
//            else if (_type.IsValueType)
//            {
//				writer.WriteLine("{0}.Save(writer);", _name);
//			} 
//        }
//
//        private void _WriteLoadMetadata (CodeWriter writer)
//        {
//			// here we use reader.ReadUint16() instead of MetadataType.SomeType, because the RealSomeType
//			// may be some subclass of SomeType.
//
//			var typeName = EditorMetaCommon.GetNestedClassName(_type);
//
//			if (EditorMetaCommon.IsFinalType(_type))
//			{
//				writer.WriteLine("{0} = ({1}) {2}.Create((ushort) MetadataType.{3});", _name, typeName, typeof(MetaFactory).Name, EditorMetaCommon.GetMetaTypeName(_type));
//			}
//			else 
//			{
//				writer.WriteLine("{0} = ({1}) {2}.Create(reader.ReadUInt16());", _name, typeName, typeof(MetaFactory).Name);
//			}
//
//            writer.WriteLine("{0}.Load(reader);", _name);
//        }
//
//        private void _WriteSaveMetadata (CodeWriter writer)
//        {
//			if (!EditorMetaCommon.IsFinalType(_type))
//			{
//				writer.WriteLine("var {0}Type = {0}.GetMetadataType();", _name);
//				writer.WriteLine("writer.Write({0}Type);", _name);
//			}
//            
//            writer.WriteLine("{0}.Save(writer);", _name);
//        }
//
//		public override void WriteNotEqualsReturn (CodeWriter writer)
//		{
//			if (_type.IsValueType)
//			{
//				writer.WriteLine("if (!{0}.EqualsTo (that.{0}))", _name);
//				using (CodeScope.CreateCSharpScope(writer))
//				{
//					writer.WriteLine("return false;");
//				}
//			}
//			else 
//			{
//				writer.WriteLine("if (null == {0} && null != that.{0} || null != {0} && !{0}.EqualsTo (that.{0}))", _name);
//				using (CodeScope.CreateCSharpScope(writer))
//				{
//					writer.WriteLine("return false;");
//				}
//			}
//		}
//    }
//}