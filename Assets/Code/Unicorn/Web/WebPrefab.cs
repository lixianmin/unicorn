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
    public class WebPrefab : Disposable, IWebNode
    {
        internal WebPrefab(WebArgument argument, Action<WebPrefab> handler)
        {
            // _webItem这里也需要同步赋值, 因为回调handler有可能是一个小时之后的事, 中间万一使用到了_webItem就可能是null了.
            // 你永远也不知道构造方法和handler谁先到来
            var node = WebManager.It.LoadAsset(argument, node =>
            {
                if (!IsDisposed())
                {
                    if (node.Asset is GameObject mainAsset)
                    {
                        var script = mainAsset.GetComponent<MbPrefabAid>();
                        if (script is null)
                        {
                            script = mainAsset.AddComponent<MbPrefabAid>();
                            script.key = argument.key;
                        }

// 实践证明这个注释没有意义，因为这个项目会编译成dll，编译时UNITY_EDITOR是有的 --> 因为引入三方库, 编译时需移除UNITY_EDITOR
// #if UNITY_EDITOR
                        // 只要是在editor中, 无论script是否为null, 都需要赋值shader
                        WebTools.ReloadShaders(mainAsset);
                        WebTools.ReloadVisualEffects(mainAsset);
// #endif

                        _aidScript = script;

                        // 添加引用计数, WebPrefab也算作一次引用计数
                        PrefabRecycler.TryAddPrefab(argument.key, this);
                        PrefabRecycler.AddReference(argument.key);

                        // 这里的handler是有可能立即调用到的, 所以不能外面new WebItem()返回值的时候设置_webItem
                        _webNode = node;
                    }
                    else
                    {
                        Logo.Warn(
                            $"node.Asset is not gameObject, argument.key={argument.key}, node.Asset={node.Asset}");
                        _webNode = EmptyWebNode.It;
                    }
                }

                CallbackTools.Handle(ref handler, this, "[WebPrefab()]");
            });

            // 如果_webItem==null，说明LoadAsset()的handler还未执行，所以在些设置一次；之所以这样做，是因为_webItem有可能已经被设置为EmptyWebNode了
            _webNode ??= node;
        }

        protected override void _DoDispose(int flags)
        {
            if (_aidScript != null)
            {
                var key = _aidScript.key;

                // WebPrefab也算作计数1次
                PrefabRecycler.RemoveReference(key);

                var allowDestroyingAssets = Application.isEditor;
                UnityEngine.Object.DestroyImmediate(_aidScript, allowDestroyingAssets);

                // Logo.Info($"[_DoDispose()] key={key}");
            }
        }

        // public override string ToString()
        // {
        //     return $"WebPrefab: id={_id.ToString()}, key={_webItem.Key}";
        // }

        public WebStatus Status => _webNode.Status;

        UnityEngine.Object IWebNode.Asset => _webNode.Asset;

        /// <summary>
        /// 返回mainAsset
        /// </summary>
        public GameObject Asset => _webNode.Asset as GameObject;

        private IWebNode _webNode;

        private MbPrefabAid _aidScript;
        // private readonly int _id = WebTools.GetNextId();
    }
}