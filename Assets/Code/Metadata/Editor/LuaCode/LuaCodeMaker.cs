
/********************************************************************
created:    2015-03-19
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Unicorn;
using Unicorn.AutoCode;

namespace Metadata.LuaCode
{
    class LuaCodeMaker
    {
		public LuaCodeMaker ()
		{
			var directory = os.path.join(UnicornManifest.GetLuaScriptRoot(), "Metadata");
            directory = os.path.normpath(directory);

			os.mkdir(directory);
			_directory = directory;
		}

		public void WriteAll ()
		{
			_WriteLuaMetadataType_Lua();
            _WriteLuaMetadataFiles();

            System.Threading.ThreadPool.QueueUserWorkItem(state =>
                {
                    _RemoveOutdatedLuaMetadataFiles();
                });
		}

        private void _RemoveOutdatedLuaMetadataFiles ()
        {
            var validLuaFiles = new HashSet<string>();
            validLuaFiles.Add(_FetchLuaMetadataTypePath());

            foreach (var type in EditorMetaTools.EnumerateLuaTypes())
            {
                var path = _FetchLuaMetadataPath(type.Name);
                validLuaFiles.Add(path);
            }

            var allLuaFiles = Directory.GetFiles(_directory, "*.lua");
            os.path.normpath(allLuaFiles);

            foreach (var filepath in allLuaFiles)
            {
                if (!validLuaFiles.Contains(filepath))
                {
                    File.Delete(filepath);
                }
            }
        }

        private void _WriteLuaMetadataFiles ()
        {
            foreach (var type in EditorMetaTools.EnumerateLuaTypes())
            {
                _Write(type);
            }
        }

        private string _FetchLuaMetadataTypePath ()
        {
            var typeName = metadataTypeName;
            var path = _directory + "/" + typeName + ".lua";
            return path;
        }

        private string _FetchLuaMetadataPath (string name)
        {
            var path = _directory + "/" + name + ".lua";
            return path;
        }

		private void _Write (MetaType type)
		{
			var name = type.Name;
            var path = _FetchLuaMetadataPath(type.Name);
			
			using (var writer = new CodeWriter(path))
			{
				_WriteHead (writer, type);

				var creatorMembers = new List<LuaBase>();
				var root = LuaBase.Create(type.RawType, name, name);
				root.CollectCreatorMembers(creatorMembers);
				
				for (int i= creatorMembers.Count - 1; i>= 0; i--)
				{
					var member = creatorMembers[i];
					member.WriteCreatorFunction(writer);
				}

				if (type.IsTemplate)
				{
                    _WriteTemplate(writer, type.RawType, name);
				}
				else if (type.IsConfig)
				{
                    _WriteConfig(writer, type.RawType, name);
				}
			}
        }

		private void _WriteHead (CodeWriter writer, MetaType type)
		{
            writer.WriteLine("local {0} = require '{1}'", metadataTypeName, _metadataTypeFullName);
			writer.WriteLine("local {0} = {{ }}", type.Name);
			
			if (type.IsTemplate)
			{
				writer.WriteLine("local cache = setmetatable({ }, MetadataType.cacheMetatable)");
			}
		}

		private void _WriteTemplate (CodeWriter writer, Type type, string name)
		{
			_WriteTemplateCreate(writer, type, name);
			_WriteTemplateEnumerateIDs(writer, type, name);

            writer.WriteLine();
			writer.WriteLine("return {0}", name);
		}

		private void _WriteTemplateCreate (CodeWriter writer, Type type, string name)
		{
            writer.WriteLine();
			writer.WriteLine("function {0}.Create (idTemplate, ignoreError)", name);
			
			using (CodeScope.CreateLuaScope(writer))
			{
				writer.WriteLine("local template = cache[idTemplate]");

				writer.WriteLine("if not template then");
				using (CodeScope.CreateLuaScope(writer))
				{
                    writer.WriteLine("local aid = {0}", typeof(Metadata.LuaLoadAid).FullName);
                    writer.WriteLine("if aid.Seek ('{0}', idTemplate) and aid.ReadBoolean() then", type.GetTypeNameEx());
					
					using (CodeScope.CreateLuaScope(writer))
					{
						writer.WriteLine("template = {0}._Create{0} (aid)", name);
                    }

                    writer.WriteLine();
					writer.WriteLine("if template then");
					using (CodeScope.Create(writer, string.Empty, "elseif not ignoreError then\n"))
					{
						writer.WriteLine("cache[idTemplate] = template");
					}
					
					using (CodeScope.CreateLuaScope(writer))
					{
						writer.WriteLine("error ('[{0}.Create()] Invalid idTemplate= '..tostring(idTemplate))", name);
                    }
                }

                writer.WriteLine();
                writer.WriteLine("return template");
			}
		}

		private void _WriteTemplateEnumerateIDs (CodeWriter writer, Type type, string name)
		{
            writer.WriteLine();
			writer.WriteLine("function {0}.GetIDs ()", name);
			
			using (CodeScope.CreateLuaScope(writer))
			{
				string ids = "ids";
				writer.WriteLine("local {0} = cache.{0}", ids);
				writer.WriteLine("if not {0} then", ids);

				using (CodeScope.Create(writer, string.Empty, "end\n\n"))
				{
                    writer.WriteLine("local aid = {0}", typeof(Metadata.LuaLoadAid).FullName);
                    writer.WriteLine("{0} = aid.EnumerateIDs ('{1}')", ids, type.GetTypeNameEx());
					writer.WriteLine("cache.{0} = {0}", ids);
                }
                
				writer.WriteLine("return {0}", ids);
            }
        }
        
        private void _WriteConfig (CodeWriter writer, Type type, string name)
		{
            writer.WriteLine();
            writer.WriteLine("local config = nil");
			writer.WriteLine("function {0}.Create ()", name);
			
			using (CodeScope.CreateLuaScope(writer))
			{
				writer.WriteLine("if not config then");
				using (CodeScope.CreateLuaScope(writer))
				{
					if (_isEnableCostTimeWarning)
					{
						writer.WriteLine("local startTime = os.clock ()");
					}

                    writer.WriteLine("local aid = {0}", typeof(Metadata.LuaLoadAid).FullName);
                    writer.WriteLine("if aid.Seek ('{0}', 0) and aid.ReadBoolean() then", type.GetTypeNameEx());
					using (CodeScope.CreateLuaScope(writer))
					{
						writer.WriteLine("config = {0}._Create{0} (aid)", name);
					}

					if (_isEnableCostTimeWarning)
					{
						writer.WriteLine("local costTime = os.clock() - startTime");
						writer.WriteLine("if costTime > 0.1 then");
						using (CodeScope.Create(writer, string.Empty, "end\n"))
						{
							writer.WriteLine("warning ('[{0}.Create ()] performance warning: costTime='.. costTime)", name);
						}
					}
				}

				writer.WriteLine("return config");
			}
			
            writer.WriteLine();
			writer.WriteLine("return {0}", name);
		}

		private void _WriteLuaMetadataType_Lua ()
		{
			var typeName = metadataTypeName;
            var path = _FetchLuaMetadataTypePath();
			
			using (var writer = new CodeWriter(path))
			{
				writer.WriteLine("local {0} = ", typeName);
				using (CodeScope.CreateCSharpScope(writer))
				{
					writer.WriteLine("cacheMetatable = { __mode = 'v' },");
                    writer.WriteLine(
    @"readonlyMetatable =
    {
        __index = function (self, k)
            error (string.format('__index invalid global variables, [ %s => nil]', tostring(k)))
        end,

        __newindex = function (self, k, v)
            rawset(self, k, v)
            warning (string.format('__newindex unexpected global variables, [ %s => %s ]', tostring(k), tostring(v)))
        end,
    }
");
				}

				writer.WriteLine("return {0}", typeName);
			}
		}

		private string _directory;
		private bool _isEnableCostTimeWarning = false;

        public const string metadataTypeName = "MetadataType";
		private const string _metadataTypeFullName = "Metadata." + metadataTypeName;
    }
}
