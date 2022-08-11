
/*********************************************************************
created:    2015-08-05
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;

namespace Unicorn
{
    //[Obfuscators.ObfuscatorIgnore]
    public static class ExtendedException
    {
        public static string ToStringEx (this Exception ex)
        {
            if (null != ex)
            {
				return string.Concat("[", ex.ToString(), "\n\n StackTrace=", ex.StackTrace, "]");
            }

            return string.Empty;
        }
    }
}