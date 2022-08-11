//
///********************************************************************
//created:    2014-01-09
//author:     lixianmin
//
//Copyright (C) - All Rights Reserved
//*********************************************************************/
//
//using System;
//using System.IO;
//using System.Reflection;
//using System.Linq;
//using System.Collections.Generic;
//using Unicorn;
//using Unicorn.AutoCode;
//
//namespace Metadata
//{
//    class DrawableObjectCodeMaker
//    {
//        public DrawableObjectCodeMaker (string directory)
//        {
//            if(!Directory.Exists(directory))
//            {
//                Directory.CreateDirectory(directory);
//            }
//
//            _directory = directory;
//        }
//
//        public void WriteAll ()
//        {
//            var metaManager = new MetaManager();
//
//            foreach(var type in metaManager.MetaTypes)
//            {
//                if(!type.IsAbstract)
//                {
//                    _WriteOneClass(type);
//                }
//            }
//        }
//
//        private void _WriteOneClass (MetaType type)
//        {
//			string drawableClassName = EditorMetaCommon.GetDrawableClassName(type.RawType);
//			string fname = string.Concat(_directory, drawableClassName, ".cs");
//            using (_writer = new CodeWriter(fname))
//            {
//                _writer.WriteLine("using UnityEngine;");
//                
//                _writer.WriteLine();
//                _writer.WriteLine("namespace Metadata.Editor");
//                using(CodeScope.CreateCSharpScope(_writer))
//                {
//                    _writer.WriteLine("public class {0} : ScriptableObject, {1}", drawableClassName, typeof(IMetadataScriptableObject).Name);
//                    using (CodeScope.CreateCSharpScope(_writer))
//                    {
//                        string className = type.Name;
//                        string classNestedName = EditorMetaCommon.GetNestedClassName(type.RawType);
//                        
//                        string memberName = char.ToLower(className [0]) + className.Substring(1);
//                        _writer.WriteLine("public {0} {1};", classNestedName, memberName);
//                        _writer.WriteLine();
//                        
//                        _writer.WriteLine("public {0} metadata", typeof(IMetadata).Name);
//                        using(CodeScope.CreateCSharpScope(_writer))
//                        {
//                            _writer.WriteLine("get");
//                            using(CodeScope.CreateCSharpScope(_writer))
//                            {
//                                _writer.WriteLine("return {0};", memberName);
//                            }
//                            
//                            _writer.WriteLine("set");
//                            using(CodeScope.CreateCSharpScope(_writer))
//                            {
//                                _writer.WriteLine("{0} = ({1}) value;", memberName, classNestedName);
//                            }
//                        }
//                    }
//                }
//            }
//        }
//
//        private string _directory;
//        private CodeWriter _writer;
//    }
//}