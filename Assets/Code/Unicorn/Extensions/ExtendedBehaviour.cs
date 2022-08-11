
/********************************************************************
created:    2014-01-04
author:     lixianmin

Copyright (C) - All Rights Reserved
 *********************************************************************/
using UnityEngine;

namespace Unicorn
{
    //[Obfuscators.ObfuscatorIgnore]
    public static class ExtendedBehaviour
    {
        public static void SetEnabledEx (this Behaviour behaviour, bool enabled)
        {
            if (null != behaviour)
            {
                behaviour.enabled = enabled;
            }
        }

        public static bool GetEnabledEx (this Behaviour behaviour)
        {
            if (null != behaviour)
            {
                return behaviour.enabled;
            }

            return false;
        }
    }
}