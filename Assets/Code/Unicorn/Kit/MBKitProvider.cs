
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
            var kit = _CreateKitByFullKitName();
            if (kit is not null)
            {
                kit.sort = sort;
                _kit = kit;
                kit._Init(transform, assets);
            }
            else
            {
                Console.Error.WriteLine("invalid fullKitName={0}, or need to make auto code", fullKitName);
            }
        }

        private void OnValidate()
        {
            // 如果是在修改fullKitName
            if (_lastFullKitName != fullKitName)
            {
                _lastFullKitName = fullKitName;
                var kit = _CreateKitByFullKitName();
                if (kit is not null)
                {
                    sort = TypeTools.SetDefaultTypeIndex(kit.GetType());
                }
            }
        }
        
        private void OnDestroy()
        {
            _kit?._Dispose();
        }

        private KitBase _CreateKitByFullKitName()
        {
            var key = (fullKitName ?? string.Empty).Trim();
            if (_lookupTable[key] is Func<KitBase> creator && creator() is { } kit)
            {
                return kit;
            }

            return null;
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

        public int sort;

        private KitBase _kit;
        private static readonly Hashtable _lookupTable;
    }
}