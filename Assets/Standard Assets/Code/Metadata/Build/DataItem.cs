
/********************************************************************
created:    2015-10-30
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.IO;
using Unicorn.IO;

namespace Metadata.Build
{
	internal class DataItem
	{
		public static DataItem Create (IMetadata metadata)
		{
			if (null == metadata)
			{
				return null;
			}

			var item = new DataItem { _metadata = metadata };
			return item;
		}

		public static DataItem Create (byte[] buffer)
		{
			if (null == buffer)
			{
				return null;
			}

			using var stream = new MemoryStream(buffer);
			using var reader = new OctetsReader(stream, OctetsMode.Compress);
			
			var typeName = reader.ReadString();
			if (string.IsNullOrEmpty(typeName))
			{
				return null;
			}

			var creator = MetaFactory.GetMetaCreator(typeName);
			var metadata = creator?.Create();
			if (null == metadata)
			{
				return null;
			}
				
			MetaTools.Load(reader, metadata);
				
			if (stream.Position != stream.Length)
			{
				return null;
			}

			var item = new DataItem { _metadata = metadata };
			return item;
		}

		public void Save (BinaryWriter masterWriter)
		{
			if (null == masterWriter)
			{
				return;
			}

			using var stream = new MemoryStream(512);
			using var writer = new OctetsWriter(stream, OctetsMode.Compress);
			var metadata = GetMetadata();
				
			var typeFullName = metadata.GetType().FullName ?? string.Empty;
			writer.Write(typeFullName);
				
			MetaTools.Save(writer, metadata, true);

			// write data buffer
			var buffer = stream.ToArray();
			var bufferLength = buffer.Length;
			masterWriter.Write(bufferLength);
			masterWriter.BaseStream.Write(buffer, 0, bufferLength);
		}

		public IMetadata GetMetadata ()
		{
			return _metadata;
		}

		private IMetadata _metadata;
	}
}