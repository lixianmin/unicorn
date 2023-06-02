//
///********************************************************************
//created:    2014-01-09
//author:     lixianmin
//
//Copyright (C) - All Rights Reserved
//*********************************************************************/
//
//using UnityEngine;
//using System;
//using System.IO;
//using System.Reflection;
//using System.Collections.Generic;
//using System.Text;
//using System.Linq;
//using Unicorn;
//using Unicorn.AutoCode;
//
//namespace Metadata.Raw
//{
//    class AutoCodeMaker
//    {
//		enum AutoCodeType
//		{
//			ClientCode,
//			StandardCode,
//			EditorCode,
//		}
//
//        public void WriteAll ()
//        {
////			_InitRootTypes(metaManager);
//			_CheckCodeDistribution();
//
////			_WriteAllMetadataClasses(metaManager);
////			_rootTypes.Clear();
//        }
//
////        private void _InitRootTypes (MetaManager metaManager)
////        {
////            foreach (var type in metaManager.MetaTypes)
////            {
////                var rootType = EditorMetaCommon.GetRootMetadata(type.RawType);
////                if (type.RawType != rootType)
////                {
////                    _rootTypes.Add(rootType);
////                }
////            }
////        }
////
////		private void _WriteAllMetadataClasses (MetaManager metaManager)
////        {
////            foreach (var type in metaManager.MetaTypes)
////            {
////                if (type.IsNested // nested class will be written in its outer class
////                    || type.IsSerializable // serializable means will write the class manually.
////                    // middle class ( not root and not the very node)
////                    || !_rootTypes.Contains(type.RawType) && type.IsAbstract)
////                {
////                    continue;
////                }
////
////				var codeAssembly = type.GetCodeAssembly();
////
////				var flags = type.GetExportFlags();
////				if (flags == ExportFlags.ExportLua && codeAssembly != CodeAssembly.EditorAssembly)
////				{
////					Logo.Error("{0} --- with [Export(ExportFlags.ExportLua)] should be under Editor folder", type.FullName);
////					continue;
////				}
////
////                using (_writer = new CodeWriter(type.GetAutoCodePath()))
////                {
////                    _WriteFileHead();
////
////					using (CodeScope.Create(_writer, "{\n", "}"))
////                    {
////                        _WriteOneClassOrStruct(type.RawType, type.GetExportFlags());
////                    }
////                }
////            }
////        }
//
//		private CodeAssembly _CheckCodeDistribution ()
//		{
//			var distribution = CodeAssembly.None;
//
//            foreach (var type in EditorMetaTools.GetMetaTypes())
//			{
//				var codeAssembly = type.GetCodeAssembly();
//				distribution |= codeAssembly;
//				if (distribution == CodeAssembly.All)
//				{
//					break;
//				}
//			}
//
//			if ((distribution & Metadata.CodeAssembly.StandardAssembly) != 0)
//			{
//				os.makedirs(EditorMetaCommon.StandardAutoCodeDirectory);
//			}
//			
//			if ((distribution & Metadata.CodeAssembly.ClientAssembly) != 0)
//			{
//				os.makedirs(EditorMetaCommon.ClientAutoCodeDirectory);
//			}
//
////			var typeDirectory = (distribution & CodeAssembly.StandardAssembly) != 0
////				? EditorMetaCommon.StandardAutoCodeDirectory : EditorMetaCommon.ClientAutoCodeDirectory;
////			var typePath = os.path.join(typeDirectory, "_MetadataType.cs");
////			_WriteMetadataTypes(typePath, metaManager);
//			
//			var factoryDirectory = (distribution & CodeAssembly.ClientAssembly) != 0
//				? EditorMetaCommon.ClientAutoCodeDirectory : EditorMetaCommon.StandardAutoCodeDirectory;
//			var factoryPath = os.path.join(factoryDirectory, "_MetadataFactory.cs");
//			_WriteMetaFactory(factoryPath);
//
//			return distribution;
//		}
//
//        private void _WriteFileHead ()
//        {
//            _writer.WriteLine("using System;");
//            _writer.WriteLine("using System.Collections;");
//            _writer.WriteLine("using Unicorn;");
//            _writer.WriteLine("using Unicorn.IO;");
//            
//            _writer.WriteLine();
//            _writer.WriteLine("namespace Metadata");
//        }
//
////		private void _WriteMetadataTypes (string fullpath, MetaManager metaManager)
////        {
////			using (_writer = new CodeWriter(fullpath))
////			{
////				_WriteFileHead();
////				
////				using (CodeScope.CreateCSharpScope(_writer))
////				{
////					_writer.WriteLine("public enum MetadataType: ushort");
////
////					using (CodeScope.CreateCSharpScope(_writer))
////					{
////						foreach (var childType in metaManager.EnumerateMetaTypes(false))
////						{
////							_writer.WriteLine("{0} = {1},", childType.GetMetaTypeName(), childType.MetaIndex);
////						}
////
////						foreach (var childType in metaManager.EnumerateMetaTypes(true))
////						{
////							_writer.WriteLine("{0} = {1},", childType.GetMetaTypeName(), childType.MetaIndex);
////						}
////					}
////				}
////			}
////        }
//
//		private void _WriteMetaFactory (string fullpath)
//        {
//			using (_writer = new CodeWriter(fullpath))
//			{
//				_WriteFileHead();
//				
//				using (CodeScope.CreateCSharpScope(_writer))
//				{
//					_writer.WriteLine("public class {0}", MetaFactory.outerFactoryName);
//					
//					using (CodeScope.CreateCSharpScope(_writer))
//					{
//						_WriteMetaFactory_GetLookupTableByType();
//					}
//				}
//			}
//        }
//
//        private IEnumerable<MetaType> _EnumerateLookupTableTypes ()
//        {
//            foreach (var childType in EditorMetaTools.GetMetaTypes())
//            {
//                if (childType.IsAbstract)
//                {
//                    continue;
//                }
//
//                var isEditorAssembly = childType.GetCodeAssembly() == CodeAssembly.EditorAssembly;
//                if (!isEditorAssembly)
//                {
//                    yield return childType;
//                }
//            }
//        }
//
//		private void _WriteMetaFactory_GetLookupTableByType ()
//		{
//            _writer.WriteLine("private static Hashtable {0} ()", MetaFactory.outerFactoryGetLookupTableByName);
//			
//			using (CodeScope.CreateCSharpScope(_writer))
//			{
//                var metaTypeCount = System.Linq.Enumerable.Count(_EnumerateLookupTableTypes());
//                _writer.WriteLine("var table = new Hashtable({0});", metaTypeCount.ToString());
//
//                foreach (var childType in _EnumerateLookupTableTypes())
//				{
//                    var rawType = childType.RawType;
//                    _writer.WriteLine("table.Add (\"{0}\", new {1}(()=> new {2}()));"
//                        , rawType.FullName
//                        , typeof(MetaCreator).Name
//                        , rawType.GetTypeNameEx()
//                    );
//				}
//				
////				using (MacroScope.CreateEditorScope(_writer.BaseWriter))
////				{
////					foreach (var childType in metaManager.EnumerateMetaTypes(true))
////					{
////                        var rawType = childType.RawType;
////                        _writer.WriteLine("table.Add (\"{0}\", new {1} (()=> (IMetadata)Activator.CreateInstance(TypeTools.SearchType(\"{0}\"))));"
////                            , rawType.FullName
////                            , typeof(MetaCreator).Name
////                        );
////					}
////				}
//				
//				_writer.WriteLine("return table;");
//			}
//		}
//		
////        private void _WriteOneClassOrStruct (Type type, ExportFlags flags)
////        {
////            var classOrStruct = type.IsClass ? "class" : "struct";
////			var sealedClass = type.IsClass && EditorMetaCommon.IsFinalType(type) ? "sealed " : string.Empty;
////
////			using (MacroScope.CreateEditorScope(_writer.BaseWriter))
////			{
////				_writer.WriteLine("[Serializable]");
////			}
////
////			_writer.WriteLine("{0}partial {1} {2}", sealedClass, classOrStruct, type.Name);
////
////            using (CodeScope.CreateCSharpScope(_writer))
////            {
////				var nestedTypes = type.GetNestedTypes(BindingFlags.Instance | BindingFlags.Public);
////                _WriteNestedTypes(nestedTypes, flags);
////
////                var members = new List<MemberBase>();
////                _CollectSerializableMembers(type, members);
////
//////                _WriteGetMetadataTypeMethod(type);
////
////				using (MacroScope.CreateEditorScope(_writer.BaseWriter))
////				{
////					_WriteEqualsToMethod(type, members);
////				}
////
////                using (MacroScope.CreateEditorScope(_writer.BaseWriter))
////                {
////                    _WriteToStringMethod(type, members);
////                }
////            }
////        }
////
////        private void _WriteNestedTypes (Type[] nestedTypes, ExportFlags flags)
////        {
////            if (nestedTypes.Length > 0)
////            {
////                Array.Sort(nestedTypes, (a, b) => a.Name.CompareTo(b.Name));
////                foreach (var nestedType in nestedTypes)
////                {
//////                    if (typeof(ILoadable).IsAssignableFrom(nestedType))
//////                    {
//////                        continue;
//////                    }
////
////                    _WriteOneClassOrStruct(nestedType, flags);
////                }
////            }
////        }
////
////        private void _WriteLoadMethod (Type type, List<MemberBase> members)
////        {
////			_WriteExportFlagsAutoCode();
////
////            if (_rootTypes.Contains(type))
////            {
////                // we must use virtual but not abstract method, because we may cut all subclass autocode and we need
////                // to make sure the code can be compiled without errors.
////				_writer.WriteLine("public virtual void Load (IOctetsReader reader)");
////                using(CodeScope.CreateCSharpScope(_writer))
////                {
////                    _writer.WriteLine("throw new NotImplementedException(\"This method should be override~\");");
////                }
////            }
////            else
////            {
////                var overrideText = _rootTypes.Contains(EditorMetaCommon.GetRootMetadata(type)) ? "override " : string.Empty;
////				_writer.WriteLine("public {0}void Load (IOctetsReader reader)", overrideText);
////                using (CodeScope.CreateCSharpScope(_writer))
////                {
////                    for (int index= 0; index < members.Count; ++index)
////                    {
////                        var member = members[index];
////                        member.WriteLoad(_writer);
////                    }
////                }
////            }
////        }
////
////        private void _WriteSaveMethod (Type type, List<MemberBase> members)
////        {
////			if (_rootTypes.Contains(type))
////            {
////				_WriteExportFlagsAutoCode();
////				_writer.WriteLine("public virtual void Save (IOctetsWriter writer)");
////                using(CodeScope.CreateCSharpScope(_writer))
////                {
////                    _writer.WriteLine("throw new NotImplementedException(\"This method should be override~\");");
////                }
////            }
////            else
////            {
////				using (MacroScope.CreateEditorScope(_writer.BaseWriter))
////				{
////					_writer.WriteLine("[Export(ExportFlags.AutoCode)]");
////
////					var overrideText = _rootTypes.Contains(EditorMetaCommon.GetRootMetadata(type)) ? "override " : string.Empty;
////					_writer.WriteLine("public {0}void Save (IOctetsWriter writer)", overrideText);
////					using (CodeScope.Create(_writer, "{\n", "}\n"))
////					{
////						for (int index= 0; index < members.Count; ++index)
////						{
////							var member = members[index];
////							member.WriteSave(_writer);
////						}
////					}
////				}
////            }
////        }
////
////        private void _WriteGetMetadataTypeMethod (Type type)
////        {
////			_WriteExportFlagsAutoCode();
////
////            if (_rootTypes.Contains(type))
////            {
////                _writer.WriteLine("public virtual ushort GetMetadataType ()");
////                using(CodeScope.CreateCSharpScope(_writer))
////                {
////                    _writer.WriteLine("throw new NotImplementedException(\"This method should be override~\");");
////                }
////            }
////            else
////            {
////                var overrideText = _rootTypes.Contains(EditorMetaCommon.GetRootMetadata(type)) ? "override " : string.Empty;
////                _writer.WriteLine("public {0}ushort GetMetadataType ()", overrideText);
////
////                using (CodeScope.CreateCSharpScope(_writer))
////                {
////					var typeName = EditorMetaCommon.GetMetaTypeName(type);
////					_writer.WriteLine("return (ushort) MetadataType.{0};", typeName);
////                }
////            }
////        }
////
////        private void _CollectSerializableMembers (Type type, List<MemberBase> members)
////        {
////            // private fields will not be serialized into .xml, so we must use public fields.
////            var flags = BindingFlags.Instance | BindingFlags.Public;
////
////            foreach (var field in type.GetFields(flags))
////            {
////                if (!EditorMetaCommon.IsAutoCodeIgnore(field))
////                {
////                    var member = MemberBase.Create(field.FieldType, field.Name);
////                    members.Add(member);
////                }
////            }
////
////			foreach (var property in EditorMetaCommon.GetExportableProperties(type, flags))
////			{
////				var member = MemberBase.Create(property.PropertyType, property.Name);
////				members.Add(member);
////			}
////        }
////
////		private void _WriteEqualsToMethod (Type type, List<MemberBase> members)
////		{
////			_writer.WriteLine("[Export(ExportFlags.AutoCode)]");
////			if (_rootTypes.Contains(type))
////			{
////				_writer.WriteLine("public virtual bool EqualsTo (IMetadata other)");
////				using(CodeScope.CreateCSharpScope(_writer))
////				{
////					_writer.WriteLine("throw new NotImplementedException(\"This method should be override~\");");
////				}
////			}
////			else
////			{
////				var overrideText = _rootTypes.Contains(EditorMetaCommon.GetRootMetadata(type)) ? "override " : string.Empty;
////				_writer.WriteLine("public {0}bool EqualsTo (IMetadata other)", overrideText);
////				
////				using (CodeScope.CreateCSharpScope(_writer))
////				{
////					var typeName = EditorMetaCommon.GetNestedClassName(type);
////
////					if (type.IsClass)
////					{
////						_writer.WriteLine("var that = other as {0};", typeName);
////						_writer.WriteLine("if (null == that)");
////						using (CodeScope.CreateCSharpScope(_writer))
////						{
////							_writer.WriteLine("return false;");
////						}
////					}
////					else
////					{
////						_writer.WriteLine("var that = ({0}) other;", typeName);
////					}
////					
////					var memberCount = members.Count;
////					
////					for (int index= 0; index < memberCount; ++index)
////					{
////						var member = members[index];
////						member.WriteNotEqualsReturn(_writer);
////					}
////					
////					_writer.WriteLine("return true;");
////				}
////			}
////		}
////
////        private void _WriteToStringMethod (Type type, List<MemberBase> members)
////        {
//////			_writer.WriteLine("[Export(ExportFlags.AutoCode)]");
////            _writer.WriteLine("public override string ToString ()");
////			using (CodeScope.Create(_writer, "{\n", "}\n"))
////            {
////                var memberCount = members.Count;
////
////                if (memberCount > 0)
////                {
////                    _sbText.Append("return string.Format(\"[");
////                    _sbText.Append(type.Name);
////                    _sbText.Append(":ToString()] ");
////
////                    for (int index= 0; index < memberCount; ++index)
////                    {
////                        _sbText.Append(members[index].GetMemberName());
////                        _sbText.Append("={");
////                        _sbText.Append(index);
////                        _sbText.Append("}");
////                        
////                        if (index < memberCount - 1)
////                        {
////                            _sbText.Append(", ");
////                        }
////                    }
////                    
////                    _sbText.Append("\", ");
////                    
////                    for (int index= 0; index < memberCount; ++index)
////                    {
////                        _sbText.Append(members[index].GetMemberName());
////                        
////                        if (index < memberCount - 1)
////                        {
////                            _sbText.Append(", ");
////                        }
////                    }
////
////                    _sbText.Append(");");
////                } 
////                else
////                {
////                    _sbText.Append("return \"[");
////                    _sbText.Append(type.Name);
////                    _sbText.Append(":ToString()]\";");
////                }
////                
////                _writer.WriteLine(_sbText.ToString());
////                _sbText.Length = 0;
////            }
////        }
////
////		private void _WriteExportFlagsAutoCode ()
////		{
////			using (MacroScope.CreateEditorScope(_writer.BaseWriter))
////			{
////				_writer.WriteLine("[Export(ExportFlags.AutoCode)]");
////			}
////		}
////
////        public CodeWriter GetCodeWriter ()
////        {
////            return _writer;
////        }
//
//        private CodeWriter _writer;
////        private StringBuilder _sbText = new StringBuilder(256);
////        private HashSet<Type> _rootTypes = new HashSet<Type>();
//    }
//}