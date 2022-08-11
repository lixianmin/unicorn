
/********************************************************************
created:    2018-03-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections.Generic;
using Unicorn;
using Unicorn.AutoCode;

namespace Metadata
{
    internal class MetaFactoryCodeMaker
    {
        public void WriteAll ()
        {
            _CheckCodeDistribution();    
        }

        private CodeAssembly _CheckCodeDistribution ()
        {
            var distribution = CodeAssembly.None;

            foreach (var type in EditorMetaTools.GetMetaTypes())
            {
                var codeAssembly = type.GetCodeAssembly();
                distribution |= codeAssembly;
                if (distribution == CodeAssembly.All)
                {
                    break;
                }
            }

            if ((distribution & Metadata.CodeAssembly.StandardAssembly) != 0)
            {
                os.makedirs(EditorMetaTools.StandardAutoCodeDirectory);
            }

            if ((distribution & Metadata.CodeAssembly.ClientAssembly) != 0)
            {
                os.makedirs(EditorMetaTools.ClientAutoCodeDirectory);
            }

            var factoryDirectory = (distribution & CodeAssembly.ClientAssembly) != 0
                ? EditorMetaTools.ClientAutoCodeDirectory : EditorMetaTools.StandardAutoCodeDirectory;
            var factoryPath = os.path.join(factoryDirectory, "_MetaFactory.cs");
            _WriteMetaFactory(factoryPath);

            return distribution;
        }

        private void _WriteFileHead ()
        {
            _writer.WriteLine("using System;");
            _writer.WriteLine("using System.Collections;");

            _writer.WriteLine();
            _writer.WriteLine("namespace Metadata");
        }

        private void _WriteMetaFactory (string fullpath)
        {
            using (_writer = new CodeWriter(fullpath))
            {
                _WriteFileHead();

                using (CodeScope.CreateCSharpScope(_writer))
                {
                    _writer.WriteLine("public class {0}", MetaFactory.outerFactoryName);

                    using (CodeScope.CreateCSharpScope(_writer))
                    {
                        _WriteMetaFactory_GetLookupTableByType();
                    }
                }
            }
        }

        private IEnumerable<MetaType> _EnumerateLookupTableTypes ()
        {
            foreach (var childType in EditorMetaTools.GetMetaTypes())
            {
                if (childType.IsAbstract)
                {
                    continue;
                }

                var isEditorAssembly = childType.GetCodeAssembly() == CodeAssembly.EditorAssembly;
                if (!isEditorAssembly)
                {
                    yield return childType;
                }
            }
        }

        private void _WriteMetaFactory_GetLookupTableByType ()
        {
            _writer.WriteLine("private static Hashtable {0} ()", MetaFactory.outerFactoryGetLookupTableByName);

            using (CodeScope.CreateCSharpScope(_writer))
            {
                var metaTypeCount = System.Linq.Enumerable.Count(_EnumerateLookupTableTypes());
                _writer.WriteLine("var table = new Hashtable({0});", metaTypeCount.ToString());

                foreach (var childType in _EnumerateLookupTableTypes())
                {
                    var rawType = childType.RawType;
                    _writer.WriteLine("table.Add (\"{0}\", new {1}(()=> new {2}()));"
                        , rawType.FullName
                        , typeof(MetaCreator).Name
                        , rawType.GetTypeNameEx()
                    );
                }

                _writer.WriteLine("return table;");
            }
        }

        private CodeWriter _writer;
    }
}