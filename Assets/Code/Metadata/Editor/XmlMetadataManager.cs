
/********************************************************************
created:    2014-09-01
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using Unicorn;

namespace Metadata
{
    public class XmlMetadataManager: MetadataManager
    {
		public XmlMetadataManager ()
        {

        }

        public void Serialize (TextWriter writer)
        {
            var metadata = new XmlMetadata();

            metadata.Templates.AddRange(_templateManager.GetAllTemplates());
            metadata.Configs.AddRange(_configManager.GetAllConfigs());

			var serializer = _GetSerializer();
			serializer.Serialize(writer, metadata);
        }

        public void Serialize (string path)
        {
            using (var writer = new StreamWriter(path))
            {
                writer.NewLine = os.linesep;
                Serialize(writer);
            }
        }

		public void ExceptWith (XmlMetadataManager other, ref SaveAid.IncrementData incrementData)
		{
			if (null == other || null == incrementData)
			{
				return;
			}

            var sbText = StringBuilderPool.Spawn();
            sbText.AppendLine("Export increment metadata:");

			var counterparts = new HashSet<IMetadata>();

			foreach (var oldTemplate in other._templateManager.GetAllTemplates())
			{
				var templateType = oldTemplate.GetType();
				var template = _templateManager.GetTemplate(templateType, oldTemplate.id);

				if (null == template)
				{
                    incrementData.AddRemovedMetadata(templateType.FullName, oldTemplate.id);
					sbText.AppendFormat("[[Remove] newTemplate=null, oldTemplate={0}]\n", oldTemplate.ToStringEx());
					continue;
				}

				counterparts.Add(template);

				if (MetaTools.IsEqual(template, oldTemplate))
				{
					_templateManager.RemoveTemplate(templateType, oldTemplate.id);
				}
				else
				{
					sbText.AppendFormat("[[Replace] newTemplate={0}, oldTemplate={1}]\n", template.ToStringEx(), oldTemplate.ToStringEx());
				}
			}

			foreach (var template in _templateManager.GetAllTemplates())
			{
				if (!counterparts.Contains(template))
				{
					sbText.AppendFormat("[[Add] newTemplate={0}, oldTemplate=null]\n", template.ToStringEx());
				}
			}

			counterparts.Clear();

			foreach (var oldConfig in other._configManager.GetAllConfigs())
			{
				var configType = oldConfig.GetType();
				var config = _configManager.GetConfig(configType);

				if (null == config)
				{
                    incrementData.AddRemovedMetadata(configType.FullName, 0);
					sbText.AppendFormat("[[Remove] newConfig=null, oldConfig={0}]\n", oldConfig.ToStringEx());
					continue;
				}

				counterparts.Add(config);

                if (MetaTools.IsEqual(config, oldConfig))
				{
					_configManager.RemoveConfig(configType);
				}
				else
				{
					sbText.AppendFormat("[[Replace] newConfig={0}, oldConfig={1}]\n", config.ToStringEx(), oldConfig.ToStringEx());
				}
			}

			foreach (var config in _configManager.GetAllConfigs())
			{
				if (!counterparts.Contains(config))
				{
					sbText.AppendFormat("[[Add] newConfig={0}, oldConfig=null]\n", config.ToStringEx());
				}
			}

            Console.WriteLine(StringBuilderPool.GetStringAndRecycle(sbText));
		}

        public bool IsEmpty ()
        {
            if (_templateManager.GetTemplateCount() > 0)
            {
                return false;
            }

            if (_configManager.GetConfigCount() > 0)
            {
                return false;
            }

            return true;
        }

        public IEnumerable<IMetadata> EnumerateMetadata ()
        {
            foreach (var template in _templateManager.GetAllTemplates())
            {
                yield return template;
            }

            foreach (var config in _configManager.GetAllConfigs())
            {
                yield return config;
            }
        }

		private XmlMetadataSerializer _GetSerializer ()
		{
			if (null == _serializer)
			{
				_serializer = new XmlMetadataSerializer();
			}
			
			return _serializer;
		}

        internal TemplateManager GetTemplateManager ()
        {
            return _templateManager;
        }

        internal ConfigManager GetConfigManager ()
        {
            return _configManager;
        }

		private XmlMetadataSerializer _serializer;
    }
}