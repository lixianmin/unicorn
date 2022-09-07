
/********************************************************************
created:    2022-08-26
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using UnityEngine;

namespace Unicorn
{
    public class MBKitProvider : MonoBehaviour
    {
        private void Awake()
        {
            var kit = KitFactory.Create(fullKitName);
            if (kit is not null)
            {
                kit.sort = TypeTools.SetDefaultTypeIndex(kit.GetType());
                _kit = kit;
                kit._Init(transform, assets);
            }
            else
            {
                Console.Error.WriteLine("invalid fullKitName={0}, or need to make auto code", fullKitName);
            }
        }

        /// <summary>
        /// 引入OnEnable(), 除了本身的功能外, 还有一个用作是激活MBKitProvider脚本在Inspector窗口的复选框
        /// </summary>
        private void OnEnable()
        {
            _kit?._Enable(isActiveAndEnabled);
        }

        private void OnDisable()
        {
            _kit?._Disable(false);
        }
        
        private void OnDestroy()
        {
            _kit?._Dispose();
        }

        /// <summary>
        /// 包含namespace的kit脚本全称, 用于生成kit脚本
        /// </summary>
        public string fullKitName;

        private string _lastFullKitName;

        /// <summary>
        /// 关联场景资源, 用于kit脚本逻辑
        /// </summary>
        public UnityEngine.Object[] assets;
        
        private KitBase _kit;
    }
}