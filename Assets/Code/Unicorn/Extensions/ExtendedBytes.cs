
/*********************************************************************
created:    2014-12-29
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn
{
    //[Obfuscators.ObfuscatorIgnore]
    public static class ExtendedBytes
    {
        public static string ToUtf8Ex (this byte[] bytes)
        {
            if (null != bytes)
            {
                var text = new string(Kernel.UTF8.GetChars(bytes, 0, bytes.Length));
                return text;
            }

            return string.Empty;
        }
    }
}