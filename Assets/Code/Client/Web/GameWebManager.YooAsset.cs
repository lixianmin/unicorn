/********************************************************************
created:    2023-05-31
author:     lixianmin

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
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
            if (os.IsReleaseMode)
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