/********************************************************************
created:    2022-08-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

#if UNICORN_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using Unicorn;
using Unicorn.Web;
using UnityEngine;
using YooAsset;

namespace Clients.Web
{
    using UObject = UnityEngine.Object;

    public class WebScene : Disposable, IWebNode
    {
        internal WebScene(WebArgument argument, Action<WebScene> handler)
        {
            argument.key ??= string.Empty;
            _argument = argument;
            _status = WebStatus.Loading;
            CoroutineManager.It.StartCoroutine(_CoLoad(argument, handler));
        }

        private IEnumerator _CoLoad(WebArgument argument, Action<WebScene> handler)
        {
            var loadHandle = YooAssets.LoadSceneAsync(argument.key);
            _loadHandle = loadHandle;

            // LoadAssetAsync发生InvalidKeyException异常时，只会打印一条日志，不会真正的抛出异常
            while (!loadHandle.IsDone)
            {
                yield return null;
            }

            // 无论加载是否成功，都需要回调到handler
            _status = loadHandle.Status == EOperationStatus.Succeed ? WebStatus.Succeeded : WebStatus.Failed;

            // 如果是editor，则处理root game objects，重新给shader赋值
            if (_status == WebStatus.Succeeded && Application.isEditor)
            {
                var scene = loadHandle.SceneObject;
                var rootObjects = new List<GameObject>(scene.rootCount);
                scene.GetRootGameObjects(rootObjects);

                foreach (var goAsset in rootObjects)
                {
                    WebTools.ReloadShaders(goAsset);
                }

                var skybox = RenderSettings.skybox;
                if (skybox is not null && skybox.shader != null)
                {
                    skybox.shader = Shader.Find(skybox.shader.name);
                }
            }

            CallbackTools.Handle(ref handler, this, string.Empty);
        }

        protected override void _DoDispose(int flags)
        {
            Logo.Info($"[_DoDispose()] _loadHandle.IsValid()={_loadHandle.IsValid} , Key={Key}");
            if (_loadHandle.IsValid)
            {
                _loadHandle.UnloadAsync();
                // Addressables.UnloadSceneAsync(_loadHandle);
            }
        }

        public string Key => _argument.key;
        public WebStatus Status => _status;

        UObject IWebNode.Asset => throw new InvalidOperationException("we do not need to implement this property");

        private readonly WebArgument _argument;
        private SceneHandle _loadHandle;
        private WebStatus _status;
    }
}

#endif