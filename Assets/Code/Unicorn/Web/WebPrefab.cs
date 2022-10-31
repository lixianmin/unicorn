
/********************************************************************
created:    2022-08-13
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using UnityEngine;
using Unicorn.Web.Internal;

namespace Unicorn.Web
{
    public partial class WebPrefab: Disposable, IWebNode
    {
        internal WebPrefab(WebArgument argument, Action<WebPrefab> handler)
        {
            // _webItem这里也需要同步赋值, 因为回调handler有可能是一个小时之后的事, 中间万一使用到了_webItem就可能是null了. 你永远也不知道构造方法和handler谁先到来
            _webItem = WebManager.Instance.LoadAsset(argument, webItem =>
            {
                if (webItem.Asset is not GameObject mainAsset) return;

                var script = mainAsset.GetComponent<MbPrefabAid>();
                if (script is null)
                {
                     script = mainAsset.AddComponent<MbPrefabAid>();
                     script.key = argument.key;
                     
#if UNITY_EDITOR
                     _ProcessDependenciesInEditor(mainAsset);
#endif
                }

                _aidScript = script;
                PrefabRecycler.TryAddPrefab(argument.key, this);

                // 这里的handler是有可能立即调用到的, 所以不能外面new WebItem()返回值的时候设置_webItem
                _webItem = webItem;
                CallbackTools.Handle(ref handler, this, "[WebPrefab()]");
            });
        }

        protected override void _DoDispose(bool isManualDisposing)
        {
            var allowDestroyingAssets = Application.isEditor;
            UnityEngine.Object.DestroyImmediate(_aidScript, allowDestroyingAssets);
            // Console.WriteLine("[_DoDispose()] {0}", this.ToString());
        }

        // public override string ToString()
        // {
        //     return $"WebPrefab: id={_id.ToString()}, key={_webItem.Key}";
        // }

        public bool IsDone => _webItem.IsDone;
        public bool IsSucceeded => _webItem.IsSucceeded;
        
        UnityEngine.Object IWebNode.Asset => _webItem.Asset;
        
        /// <summary>
        /// 返回mainAsset
        /// </summary>
        public GameObject Asset => _webItem.Asset as GameObject;
        
        private IWebNode _webItem;
        private MbPrefabAid _aidScript;
        // private readonly int _id = WebTools.GetNextId();
    }
}