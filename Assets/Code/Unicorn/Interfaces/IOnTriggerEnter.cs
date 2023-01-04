/*********************************************************************
created:    2023-01-04
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;

namespace Unicorn
{
    public interface IOnTriggerEnter
    {
        void OnTriggerEnter(Collider other);
    }
}