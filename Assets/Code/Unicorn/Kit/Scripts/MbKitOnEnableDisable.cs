/*********************************************************************
created:    2023-01-04
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;

namespace Unicorn.Kit
{
    internal class MbKitOnEnableDisable : MonoBehaviour
    {
        /// <summary>
        /// 在 gameObject.SetActive(true) 或 script.enabled=true 时都会触发OnEnable()事件
        /// </summary>
        private void OnEnable()
        {
            kit.isActiveAndEnabled = true;
            if (kit is IOnEnable item)
            {
                CallbackTools.Handle(item.OnEnable, "[OnEnable()]");
            }
        }

        /// <summary>
        /// 在 gameObject.SetActive(false) 或 script.enabled=false 时都会触发OnDisable()事件
        /// </summary>
        private void OnDisable()
        {
            kit.isActiveAndEnabled = false;
            if (kit is IOnDisable item)
            {
                CallbackTools.Handle(item.OnDisable, "[OnDisable()]");
            }
        }

        public KitBase kit;
    }
}