
/*********************************************************************
created:	2017-01-01
author:		lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.IO;

namespace Unicorn
{
	public static class ExtendedTextReader
	{
		public static string GetDecodedBufferEx (this TextReader reader)
		{
			if (null == reader)
			{
				return string.Empty;
			}

			var decoded_buffer_field = reader.GetType().GetField("decoded_buffer",
				System.Reflection.BindingFlags.Instance
				| System.Reflection.BindingFlags.GetField
				| System.Reflection.BindingFlags.NonPublic);

			char[] decoded_buffer = decoded_buffer_field.GetValue(reader) as char[];
			string bufferText = new string(decoded_buffer);

			return bufferText;
		}
	}
}