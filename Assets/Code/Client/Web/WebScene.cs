/********************************************************************
created:    2022-08-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Unicorn;
using Unicorn.Web;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Client.Web
{
    using UObject = UnityEngine.Object;

    public class WebScene : Disposable, IWebNode
    {
        internal WebScene(WebArgument argument, Action<WebScene> handler)
        {
            argument.key ??= string.Empty;
            _argument = argument;
            CoroutineManager.Instance.StartCoroutine(_CoLoad(argument, handler));
        }

        private IEnumerator _CoLoad(WebArgument argument, Action<WebScene> handler)
        {
            var loadHandle = Addressables.LoadSceneAsync(argument.key);
            _loadHandle = loadHandle;

            // LoadAssetAsync发生InvalidKeyException异常时，只会打印一条日志，不会真正的抛出异常
            while (!loadHandle.IsDone)
            {
                yield return null;
            }

            // 无论加载是否成功，都需要回调到handler
            IsDone = true;
            IsSucceeded = loadHandle.Status == AsyncOperationStatus.Succeeded;

#if UNITY_EDITOR
            // 如果是editor，则处理root game objects，重新给shader赋值
            if (IsSucceeded && Application.isEditor)
            {
                var scene = loadHandle.Result.Scene;
                var rootObjects = new List<GameObject>(scene.rootCount);
                scene.GetRootGameObjects(rootObjects);

                foreach (var goAsset in rootObjects)
                {
                    WebTools.ReloadShaders(goAsset);
                }

                var skybox = RenderSettings.skybox;
                if(skybox is not null && skybox.shader != null)
                {
                    skybox.shader = Shader.Find(skybox.shader.name);
                }
            }
#endif
            CallbackTools.Handle(ref handler, this, string.Empty);
        }

        protected override void _DoDispose(bool isManualDisposing)
        {
            Addressables.Release(_loadHandle);
            // Console.WriteLine("[_DoDispose()] {0}", this.ToString());
        }

        public bool IsDone { get; private set; }
        public bool IsSucceeded { get; private set; }
        public string Key => _argument.key;

        UObject IWebNode.Asset => throw new InvalidOperationException("we do not need to implement this property");

        private readonly WebArgument _argument;
        private AsyncOperationHandle<SceneInstance> _loadHandle;
    }
}