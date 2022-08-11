
/********************************************************************
created:    2014-08-30
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn
{
	//[Obfuscators.ObfuscatorIgnore]
    public static class ExtendedMemoryFormatter
    {
        public static string ToStringKB (this int memorySize)
        {
            return (memorySize / 1024.0f).ToString("F1") + " KB";
        }

        public static string ToStringMB (this int memorySize)
        {
			return (memorySize / 1048576.0f).ToString("F1") + " MB";
        }

		public static string ToStringHB (this int memorySize)
		{
			return memorySize >= 1048576 ? ToStringMB(memorySize) : ToStringKB(memorySize);
		}

        public static string ToStringKB (this long memorySize)
        {
            return (memorySize / 1024.0f).ToString("F1") + " KB";
        }
        
        public static string ToStringMB (this long memorySize)
        {
			return (memorySize / 1048576.0f).ToString("F1") + " MB";
        }

		public static string ToStringHB (this long memorySize)
		{
			return memorySize >= 1048576 ? ToStringMB(memorySize) : ToStringKB(memorySize);
		}
    }
}