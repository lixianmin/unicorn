/********************************************************************
created:    2023-05-31
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
#if UNICORN_EDITOR

using System.Collections;
using Unicorn;
using YooAsset;

namespace Clients.Web
{
    partial class GameWebManager
    {
        public IEnumerator InitPackage()
        {
            YooAssets.Initialize();
            YooAssets.SetOperationSystemMaxTimeSlice(30);
            
            var playMode = EPlayMode.EditorSimulateMode;
            if (os.isReleaseMode)
            {
                playMode = EPlayMode.OfflinePlayMode;
            }

            // 创建默认的资源包
            const string packageName = "DefaultPackage";
            var package = YooAssets.TryGetPackage(packageName);
            if (package == null)
            {
                package = YooAssets.CreatePackage(packageName);
                YooAssets.SetDefaultPackage(package);
            }

            // 编辑器下的模拟模式
            InitializationOperation initializationOperation = null;
            if (playMode == EPlayMode.EditorSimulateMode)
            {
                // 这个在AssetBundle Builder这个editor中可以选
                const string buildPipelineName = "BuiltinBuildPipeline";
                var createParameters = new EditorSimulateModeParameters
                {
                    SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(buildPipelineName, packageName)
                };
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // 单机运行模式
            if (playMode == EPlayMode.OfflinePlayMode)
            {
                var createParameters = new OfflinePlayModeParameters();
                // createParameters.DecryptionServices = new GameDecryptionServices();
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // // 联机运行模式
            // if (playMode == EPlayMode.HostPlayMode)
            // {
            //     string defaultHostServer = GetHostServerURL();
            //     string fallbackHostServer = GetHostServerURL();
            //     var createParameters = new HostPlayModeParameters();
            //     createParameters.DecryptionServices = new GameDecryptionServices();
            //     createParameters.BuildinQueryServices = new GameQueryServices();
            //     createParameters.DeliveryQueryServices = new DefaultDeliveryQueryServices();
            //     createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            //     initializationOperation = package.InitializeAsync(createParameters);
            // }

            yield return initializationOperation;
        }
    }
}

#endif