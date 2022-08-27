
/********************************************************************
created:    2022-08-26
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace Unicorn
{
    public class MBKitProvider : MonoBehaviour
    {
        static MBKitProvider()
        {
            var kitFactoryType = TypeTools.SearchType("Unicorn._KitFactory");
            if (null != kitFactoryType)
            {
                TypeTools.CreateDelegate(kitFactoryType, "_GetLookupTableByName", out Func<Hashtable> method);
                if (method != null)
                {
                    _lookupTable = method();
                }
            }

            _lookupTable ??= new Hashtable();
        }
        
        private void Awake()
        {
            var key = (fullKitName ?? string.Empty).Trim();
            if (_lookupTable[key] is Func<KitBase> creator && creator() is { } kit)
            {
                _kit = kit;
                kit._SetAssets(assets);
                CallbackTools.Handle(kit.Awake, "[Awake()]");
            }
            else
            {
                Console.Error.WriteLine("invalid fullKitName={0}, or need to make auto code", fullKitName);
            }
        }
        
        private void OnDestroy()
        {
            var kit = _kit;
            if (kit is not null)
            {
                CallbackTools.Handle(kit.OnDestroy, "[OnDestroy()]");
                kit.RemoveAllListeners();
            }
        }

        private void Update()
        {
            var kit = _kit;
            if (kit is not null)
            {
                CallbackTools.Handle(kit.Update, "[Update()]");
            }
        }
        
        /// <summary>
        /// 包含namespace的kit脚本全称, 用于生成kit脚本
        /// </summary>
        public string fullKitName;
        
        /// <summary>
        /// 关联场景资源, 用于kit脚本逻辑
        /// </summary>
        public UnityEngine.Object[] assets;

        private KitBase _kit;
        private static readonly Hashtable _lookupTable;
    }
}