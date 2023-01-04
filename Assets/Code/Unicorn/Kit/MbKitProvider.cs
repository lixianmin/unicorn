/********************************************************************
created:    2022-08-26
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using UObject = UnityEngine.Object;
// ReSharper disable SuspiciousTypeConversion.Global

namespace Unicorn.Kit
{
    public class MbKitProvider : MonoBehaviour
    {
        private void Awake()
        {
            var kit = KitFactory.Create(fullKitName);
            if (kit is not null)
            {
                kit.sort = TypeTools.SetDefaultTypeIndex(kit.GetType());
                _kit = kit;
                kit.InnerInit(transform, assets);
                _CheckInterfaces(kit);
            }
            else
            {
                Console.Error.WriteLine("invalid fullKitName={0}, or need to make auto code", fullKitName);
            }
        }

        private void _CheckInterfaces(KitBase kit)
        {
            var go = gameObject;
            if (kit is IOnEnable or IOnDisable)
            {
                go.AddComponent<MbKitOnEnableDisable>().kit = kit;
            }
            
            if (kit is IOnTriggerEnter or IOnTriggerExit)
            {
                go.AddComponent<MbKitOnTriggerEnterExit>().kit = kit;
            }
        }

        // /// <summary>
        // /// 引入OnEnable(), 除了本身的功能外, 还有一个用作是激活MBKitProvider脚本在Inspector窗口的复选框
        // /// </summary>
        // private void OnEnable()
        // {
        //     _kit?.InnerEnable(isActiveAndEnabled);
        // }
        //
        // private void OnDisable()
        // {
        //     _kit?.InnerDisable(false);
        // }

        private void OnDestroy()
        {
            _kit?.InnerDispose();
        }

        /// <summary>
        /// 包含namespace的kit脚本全称, 用于生成kit脚本
        /// </summary>
        public string fullKitName;

        private string _lastFullKitName;

        /// <summary>
        /// 关联场景资源, 用于kit脚本逻辑
        /// </summary>
        public UObject[] assets;

        private KitBase _kit;
    }
}