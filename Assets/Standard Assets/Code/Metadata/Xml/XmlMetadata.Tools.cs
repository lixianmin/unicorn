
/********************************************************************
created:    2015-10-24
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.IO;
using System.Globalization;
using Unicorn;

namespace Metadata
{
    partial class XmlMetadata
    {
		public static string GetTypeName (string xmlPath)
		{
			if (string.IsNullOrEmpty(xmlPath) || !File.Exists(xmlPath))
			{
				return string.Empty;
			}

			var typeFlag = "xsi:type=\"";
			using (var reader = new StreamReader(xmlPath))
			{
				while (true)
				{
					var line = reader.ReadLine();
					if (string.IsNullOrEmpty(line))
					{
//						Console.Warning.WriteLine("Empty metdata file: {0}", xmlPath);
						return string.Empty;
					}
					
					var index = line.IndexOf(typeFlag, StringComparison.Ordinal);
					if (index != -1)
					{
						var typeFlagLength = typeFlag.Length;
						var startIndex = index + typeFlagLength;
						var length = line.Length - startIndex - 2;
						var typeName = line.Substring(startIndex, length);
						return typeName;
					}
				}
			}
		}
		
		public static void Serialize (string xmlFile, XmlMetadata metadata)
		{
			var lpfnSerialize = _GetSerializeFunc ();
			if (null == lpfnSerialize)
			{
				Logo.Error("lpfnSerialize is null.");
				return;
			}

			try
			{
				using (var writer = new StreamWriter(xmlFile))
				{
					lpfnSerialize(writer, metadata);
				}
			}
			catch (Exception ex)
			{
				Logo.Error("xmlFile={0}, ex={1}", xmlFile, ex);
			}
		}

		public static XmlMetadata Deserialize (string xmlFile)
		{
			var lpfnDeserialize = _GetDeserializeFunc ();
			if (null == lpfnDeserialize)
			{
				Logo.Error("lpfnDeserialize is null.");
				return null;
			}

			StreamReader reader = null;
			try
			{
				reader = new StreamReader (xmlFile);
			}
			catch (Exception ex)
			{
				Logo.Error("xmlFile={0}, ex={1}", xmlFile, ex);
				if (reader != null)
				{
					reader.Dispose();
				}
			}

			try
			{
				var metadata = lpfnDeserialize(reader);
				return metadata;
			}
			catch (InvalidOperationException ex)
			{
				var innerException = ex.InnerException;
				if (null == innerException
				    || !innerException.Message.StartsWith("The specified type was not recognized", CompareOptions.Ordinal))
				{
					Logo.Error("xmlFile={0}, ex={1}", xmlFile, ex);
				}
                else
                {
                    Logo.Error("InvalidOperationException : xmlFile={0}, ex={1}", xmlFile, ex);
                }

				if (reader != null)
				{
					reader.Dispose();
				}
			}
			catch (Exception ex)
			{
				string bufferText = reader.GetDecodedBufferEx();
				Logo.Error("xmlFile={0}, ex=\n{1}\n, bufferText=\n{2}\n", xmlFile, ex, bufferText);
                if (reader != null)
                {
                    reader.Dispose();
				}
			}

			return null;
		}

		private static Func<TextReader, XmlMetadata> _GetDeserializeFunc ()
		{
			if (null == _lpfnDeserialize)
			{
				TypeTools.CreateDelegate(_GetSerializer(), "Deserialize", out _lpfnDeserialize);
			}
			
			return _lpfnDeserialize;
		}

		private static Action<TextWriter, XmlMetadata> _GetSerializeFunc ()
		{
			if (null == _lpfnSerialize)
			{
				TypeTools.CreateDelegate(_GetSerializer(), "Serialize", out _lpfnSerialize);
			}
			
			return _lpfnSerialize;
		}

		private static object _GetSerializer ()
		{
			if (null == _serializer)
			{
				var serializerType = TypeTools.SearchType("Metadata.XmlMetadataSerializer");
				if (null != serializerType)
				{
					_serializer = Activator.CreateInstance(serializerType);
				}
			}
			
			return _serializer;
		}
        
		private static object _serializer;
		private static Action<TextWriter, XmlMetadata> _lpfnSerialize;
		private static Func<TextReader, XmlMetadata> _lpfnDeserialize;
    }
}