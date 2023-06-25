
/********************************************************************
created:    2014-08-28
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using Unicorn;

namespace Metadata
{
    public class XmlMetadataSerializer
    {
        public XmlMetadataSerializer ()
        {
//            var metaManager = new MetaManager();
            var extraTypes = (from type in EditorMetaTools.GetMetaTypes() select type.RawType).ToArray();

            var overrides = new XmlAttributeOverrides();
//            _BuildAttributesForLocaleText(overrides);
            _BuildAttributesForAutoCodeIgnore(overrides);

            _serializer = new XmlSerializer(typeof(XmlMetadata), overrides, extraTypes, null, null);
			_serializer.UnknownElement += _OnDeserializeUnknownElement;
        }

        private void _OnDeserializeUnknownElement (object sender, XmlElementEventArgs e)
        {
			var element = e.Element;
			if (null == element)
			{
				return;
			}

			var elementName = element.Name ?? string.Empty;
			if (_unknownElementNames.Contains(elementName))
			{
				return;
			}

			_unknownElementNames.Add(elementName);
			var target = e.ObjectBeingDeserialized;
			if (null != target)
			{
				const int minReportDistance = 3;
				var typeMembers = target.GetType().GetMembers();

				foreach (var member in typeMembers)
				{
					var memberName = member.Name;
					if (elementName.GetEditDistance(memberName) < minReportDistance)
					{
						Logo.Error("unknown={0}, maybe={1}, LineNumber={2} \n\n ObjectBeingDeserialized={3} \n\n clipText={4}"
							, elementName
							, memberName
							, e.LineNumber.ToString()
							, target
							, _currentReader.GetDecodedBufferEx());
						break;
					}
				}
			}
			else
			{
				Logo.Error("unknown={0}, LineNumber={1}, ObjectBeingDeserialized=null \n\n clipText={2}"
					, elementName
					, e.LineNumber.ToString()
					, _currentReader.GetDecodedBufferEx());
			}
        }

//        private void _BuildAttributesForLocaleText (XmlAttributeOverrides overrides)
//        {
////			overrides.Add(typeof(LocaleText), "i18n", new XmlAttributes{ XmlAttribute = new XmlAttributeAttribute() });
//			overrides.Add(typeof(LocaleText), "guid", new XmlAttributes{ XmlAttribute = new XmlAttributeAttribute() });
//
//            var attributes_text = new XmlAttributes
//            {
//                XmlText = new XmlTextAttribute()
//            };
//
//            overrides.Add(typeof(LocaleText), "text", attributes_text);
//        }

        private void _BuildAttributesForAutoCodeIgnore (XmlAttributeOverrides overrides)
        {
            // private fields will not be serialized into .xml, so we must use public fields.
            var flags = BindingFlags.Instance | BindingFlags.Public;

            foreach (var type in EditorMetaTools.GetMetaTypes())
            {
                foreach (var field in type.RawType.GetFields(flags))
                {
                    if (EditorMetaTools.IsAutoCodeIgnore(field))
                    {
                        var attributes = new XmlAttributes
                        {
	                        XmlIgnore = true
                        };
                        overrides.Add(type.RawType, field.Name, attributes);
                    }
                }
            }
        }
        
        public void Serialize (TextWriter writer, XmlMetadata metadata)
        {
            _serializer.Serialize(writer, metadata);
        }

        public XmlMetadata Deserialize (TextReader reader)
        {
			_currentReader = reader;
            XmlMetadata metadata = null;

			try
			{
                _unknownElementNames.Clear();
				metadata = _serializer.Deserialize(reader) as XmlMetadata;
			}
			finally
			{
				_currentReader = null;
			}

			return metadata;
        }

		private TextReader _currentReader;
        private readonly XmlSerializer _serializer;
		private readonly HashSet<string> _unknownElementNames = new HashSet<string>();
    }
}