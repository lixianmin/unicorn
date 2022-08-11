
/********************************************************************
created:    2015-10-29
author:     lixianmin

purpose:    assert
Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.IO;
using System.Xml.Serialization;

namespace Unicorn
{
	internal static class EditorXmlTools
	{
		private static void _Serialize(Type type, TextWriter writer, object item)
		{
			if (null != writer && null != type && null != item)
			{
				var serializer = new XmlSerializer(type);
				serializer.Serialize(writer, item);
			}
		}

		private static object _Deserialize(Type type, TextReader reader)
		{
			if (null != type && null != reader)
			{
				var serializer = new XmlSerializer(type);
				return serializer.Deserialize(reader);
			}

			return null;
		}
	}
}