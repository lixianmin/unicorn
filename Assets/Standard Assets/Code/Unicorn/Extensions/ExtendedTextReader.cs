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
        public static string GetDecodedBuffer(this TextReader reader)
        {
            if (null == reader)
            {
                return string.Empty;
            }

            var decodedBufferField = reader.GetType().GetField("decoded_buffer",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.GetField
                | System.Reflection.BindingFlags.NonPublic);

            if (decodedBufferField != null)
            {
                var decodedBuffer = decodedBufferField.GetValue(reader) as char[];
                var bufferText = new string(decodedBuffer);

                return bufferText;
            }

            return string.Empty;
        }
    }
}