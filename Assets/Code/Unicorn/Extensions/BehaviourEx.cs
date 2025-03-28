/********************************************************************
created:    2014-01-04
author:     lixianmin

Copyright (C) - All Rights Reserved
 *********************************************************************/

using UnityEngine;

namespace Unicorn
{
    public static class BehaviourEx
    {
        public static void SetEnabled(this Behaviour behaviour, bool enabled)
        {
            if (behaviour != null)
            {
                behaviour.enabled = enabled;
            }
        }

        // public static bool GetEnabled (this Behaviour behaviour)
        // {
        //     return behaviour is not null && behaviour.enabled;
        // }
    }
}