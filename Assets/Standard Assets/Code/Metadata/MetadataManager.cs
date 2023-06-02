
/********************************************************************
created:    2014-03-29
author:     lixianmin

禍兮福之所倚 福兮禍之所伏

MetadataManager的单键用法，是一种非常尴尬却又无奈的抉择：

1. 它必须产生子类，因为在不同的编辑器中，子类需要做出特定的处理方式；

2. 它无法强制子类做什么，无法强制子类是一个单键，无法强制子类对它的Instance变量赋值；

3. 它必须负责起单键的责任，因为在Game与Editor体系的公共代码中，只有MetadataManager.Instance
   是可用的，你不能使用GameMetadataManger.Instance或EditorMetadataManager.Instance；
   
4. 它定义protected的构造方法就是为了让client必须生成自己的子类，否则该类无法使用；

5. 它自己不生成默认的Instance对象（虽然它自己本身也是可用的），就是确保如果client不为
   Instance赋值的话，在调用Instance时会立即出错，从很快修正错误；

6. 为什么需要metadataType这样一个ushort的类型存在，为什么不能使用反射出的field.Type直接创建metadata对象
   答：原因是，反射得到的类型有可能是某个abstract的基类，不是实际的对象类型，无法用于metadata对象创建. 
   ------ 新的系统中，尝试使用type.FullName来存储对象类型，因为建立了对string的intern机制，估计效率还可以；

7. C#可以依赖反射加载，但lua实现中必须要记忆数据的结构，这样才能实现动态添加新的类型；

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using Metadata.Build;
using Unicorn;

namespace Metadata
{
    public partial class MetadataManager: IDisposable
    {
	    protected MetadataManager ()
        {
	        Instance = this;
        }

		public virtual Template GetTemplate (Type templateType, int idTemplate)
		{
			var aid = GetLoadAid();
			if (null == templateType || null == aid)
			{
				return null;
			}

			var templateManager = GetTemplateManager();
			var table = templateManager.FetchTemplateTable(templateType);

            var templateIndex = table.TryIndexValue(idTemplate, out Template template);
            if (templateIndex >= 0)
			{
				return template;
			}

            if (!table.IsCompleted)
            {
	            var typeName = templateType.FullName;
	            var creator = MetaFactory.GetMetaCreator(typeName);
	            if (null != creator)
	            {
		            if (aid.Seek(typeName, idTemplate))
		            {
			            var metadata = creator.Create();
			            MetaTools.Load(aid, metadata);
			            template = metadata as Template;
		            }

		            table.InsertByIndex(~templateIndex, idTemplate, template);
		            return template;
	            }
            }

            return null;
		}
		
		public virtual TemplateTable GetTemplateTable (Type templateType)
		{
            if (null == templateType)
			{
				return null;
			}
			
			var templateManager = GetTemplateManager();
            var table = templateManager.FetchTemplateTable(templateType);
			if (table.IsCompleted)
			{
				return table;
			}

            var aid = GetLoadAid();
            if (null != aid)
            {
                var typeName = templateType.FullName;
                aid.LoadTemplates(typeName, table);
            }
			
			return table;
		}

		public virtual Config GetConfig (Type configType)
		{
			if (null == configType)
			{
				return null;
			}
			
			var aid = GetLoadAid();
			var configManager = GetConfigManager();
			var config = configManager.GetConfig(configType);
			
			if (null == config && null != aid)
			{
                var typeName = configType.FullName;
                var creator = MetaFactory.GetMetaCreator(typeName);
                if (null != creator && aid.Seek(typeName, 0))
                {
                    var metadata = creator.Create();
                    MetaTools.Load(aid, metadata);

                    config = metadata as Config;
                    configManager.AddConfig(config);
                    return config;
                }
			}
			
			return config;
		}

        public virtual void Dispose ()
        {
            _templateManager.Dispose();
            _configManager.Dispose();

			_loadAid.Dispose();
        }

        public virtual void Clear ()
        {
            _templateManager.Clear();
            _configManager.Clear();
        }

		public void Deserialize (string xmlPath)
		{
			var metadata = XmlMetadata.Deserialize(xmlPath);
			if (null != metadata)
			{
				AddMetadata(metadata);
			}
		}

		public override string ToString ()
		{
			var sbText = new System.Text.StringBuilder();
			sbText.AppendLine("------------templates----------");
			sbText.Append(_templateManager);
			sbText.AppendLine("------------configs----------");
			sbText.Append(_configManager);

			var text = sbText.ToString();
			return text;
		}

        public LoadAid GetLoadAid ()
		{
			return _loadAid;
		}

		internal TemplateManager GetTemplateManager ()
		{
			return _templateManager;
		}
		
		internal ConfigManager GetConfigManager ()
		{
			return _configManager;
		}

		public void EnableXmlMetadata ()
		{
			if (!_isXmlMetadata)
			{
				_isXmlMetadata = true;
				_LoadBuiltFile();
			}
		}

		private void _LoadBuiltFile ()
		{
			var builtFile = new MetadataBuiltFile();
			builtFile.Build();

            var version = builtFile.GetMetadataVersion();
            _SetMetadataVersion(version);

			var tableCache = new Hashtable();
			foreach (var metadata in builtFile.EnumerateMetadata())
			{
				switch (metadata)
				{
					case Template template:
					{
						var templateType = template.GetType();
						if (tableCache[templateType] is not TemplateTable table)
						{
							table = _templateManager.FetchTemplateTable(templateType);
							tableCache[templateType] = table;
						}

						table._Append(template.id, template);
						continue;
					}
					case Config config:
						_configManager.AddConfig(config);
						break;
				}
			}
            Logo.Info("--> add metadata to _templateManager & _configManager done");

            foreach (var pair in _templateManager.EnumerateTemplateTables())
            {
                var table = pair.Value;
                table._Sort();
            }
            Logo.Info("--> sort template tables done");

			var reader = MetaFactory.CreateChunkReader(this, true);
            Logo.Info("--> CreateChunkReader() done");

			var aid = GetLoadAid();
			aid.Load(reader, false, out _metadataVersion);
            Logo.Info("--> aid.Load() done");
		}

        private void _SetMetadataVersion (int version)
        {
            _metadataVersion = version;
        }

		public int GetMetadataVersion ()
		{
			return _metadataVersion;
		}

		/// <summary>
		/// 设计为singleton而不是static类，是为了给未来的自己一个机会：万一client端需要重写一些方法呢？
		/// 2022-10-22 原本Instance是由子类负责设置的，综合考虑后决定由基类自己做这件事，理由包括：
		///     1. 把权利收回来，并且避免子类忘记
		///     2. 因为子类需提供更多的方法，方便起见会添加：public new static readonly GameWebManager Instance = new(); 性能也会比基类中Property版的更好一些
		///     3. 设计中，这就是一个singleton，不应该同时存在多个instance
		///     4. 但是，Instance对象是lazy load的，如果基类的Instance先于子类的调用了，会报NullReferenceException
		/// </summary>
		public static MetadataManager Instance { get; private set; }
		public bool IsXmlMetadata => _isXmlMetadata;

		private readonly LoadAid _loadAid = new ();

		private bool  _isXmlMetadata;
		private int _metadataVersion;
		
		protected readonly TemplateManager _templateManager = new ();
        protected readonly ConfigManager   _configManager = new ();
    }
}