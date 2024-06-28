/********************************************************************
created:    2013-12-23
author:     lixianmin

1. Resources.UnloadUnusedAssets()這個方法不能在游戲過程中調用，只能寫在場景切換
的時候，這個方法在iPad-mini2上會消耗80+ms，而在iPhone4s上會消耗180+ms

2. The reason I remove OnTick event:
   It seems be more suitable to use a coroutine to replace OnTick event,
   a coroutine is more cheaper ( less gcalloc, 16B vs. 52B ), and more maintainable.

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using UnityEngine;
using Unicorn.UI;
using UnityEngine.LowLevel;

namespace Unicorn
{
    internal static class UnicornMain
    {
        [RuntimeInitializeOnLoadMethod]
        private static void _OnInit()
        {
            // Default value on Android:
            // Max workerThreads :60  completionPortThreads:30
            // Min workerThreads:4  completionPortThreads:4
//			const int maxWorkerThreads = 8;
//			const int maxCompletionPortThreads = 8;
//			ThreadPool.SetMaxThreads(maxWorkerThreads, maxCompletionPortThreads);

            _lastSlowUpdateTime = Time.time;
            _nextSlowUpdateTime = Time.time;

            os.Init();

            // init log flags, must be after os.Init()
            Logo.Init();
            LogFile.Init();

            var persistentDataPath = Application.persistentDataPath;
            var now = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            Logo.Info(
                $"[UnicornMain.Init()]\n{now}\nplatform={Application.platform}\nos={SystemInfo.operatingSystem}\n" +
                $"deviceModel={SystemInfo.deviceModel}\nprocessorType={SystemInfo.processorType}\nprocessorCount={SystemInfo.processorCount}\n" +
                $"systemMemorySize={SystemInfo.systemMemorySize}\ngraphicsDeviceName={SystemInfo.graphicsDeviceName}\n" +
                $"graphicsDeviceVendor={SystemInfo.graphicsDeviceVendor}\ngraphicsDeviceVersion={SystemInfo.graphicsDeviceVersion}\n" +
                $"graphicsShaderLevel={SystemInfo.graphicsShaderLevel}\ngraphicsMemorySize={SystemInfo.graphicsMemorySize}\n" +
                $"logPath={PathTools.LogPath}\ndataPath={Application.dataPath}\nstreamingAssetsPath={Application.streamingAssetsPath}\n" +
                $"persistentDataPath={persistentDataPath}\nresolution={Screen.width}x{Screen.height}\nScreen.dpi={Screen.dpi:F2}\n" +
                $"supportsInstancing={SystemInfo.supportsInstancing}\nsupportsComputeShaders={SystemInfo.supportsComputeShaders}\n"
            );

            if (string.IsNullOrEmpty(persistentDataPath))
            {
                Logo.Error("Android Error: persistentDataPath is empty, please restart the Android System.");
            }

            System.Net.ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;

            _InsertPlayerLoop();
            Application.quitting += _OnApplicationQuit;
        }

        private static void _InsertPlayerLoop()
        {
            var lastSystem = PlayerLoop.GetCurrentPlayerLoop();
            var nextSystem = new PlayerLoopSystem
            {
                subSystemList = new[]
                {
                    new()
                    {
                        updateDelegate = _ExpensiveUpdate,
                        type = typeof(UnicornMain)
                    },

                    lastSystem,

                    new()
                    {
                        updateDelegate = _LateUpdate,
                        type = typeof(UnicornMain)
                    }
                }
            };

            PlayerLoop.SetPlayerLoop(nextSystem);
        }

        /// <summary>
        /// 实际上就是Update()，之所以起名ExpensiveUpdate()，是为了让使用者郑重考虑是否启用这个可能会比较费的更新逻辑
        /// </summary>
        private static void _ExpensiveUpdate()
        {
            var time = Time.time;
            var deltaTime = Time.deltaTime;

            UpdateTools.ExpensiveUpdate();
            Logo.ExpensiveUpdate();
            LogFile.ExpensiveUpdate();

            _coroutineManager.ExpensiveUpdate();
            _partUpdateSystem.ExpensiveUpdate(deltaTime);
            // _kitManager.ExpensiveUpdate(deltaTime);
            _uiManager.ExpensiveUpdate(deltaTime);


            // 慢速帧
            if (time >= _nextSlowUpdateTime)
            {
                var slowDeltaTime = time - _lastSlowUpdateTime;
                _lastSlowUpdateTime = _nextSlowUpdateTime;
                _nextSlowUpdateTime = time + 0.1f;

                _SlowUpdate(slowDeltaTime);
            }
        }

        private static void _LateUpdate()
        {
            // 这个不能放到UnicornMain.ExpensiveUpdate()中, 否则看不见
            _instanceManager.LateUpdate();

            DisposableRecycler.Update();
        }

        /// <summary>
        /// 慢速帧，约10fps，可以节约CPU
        /// </summary>
        /// <param name="deltaTime">两帧之间的时间间隔，远大于Time.deltaTime</param>
        private static void _SlowUpdate(float deltaTime)
        {
            _uiManager.SlowUpdate(deltaTime);
        }

        /// <summary>
        /// 游戏从play状态退出
        /// </summary>
        private static void _OnApplicationQuit()
        {
            _coroutineManager.Clear();
        }

        private static readonly PartUpdateSystem _partUpdateSystem = new();
        private static readonly CoroutineManager _coroutineManager = CoroutineManager.It;
        private static readonly UIManager _uiManager = UIManager.It;
        private static readonly InstanceManager _instanceManager = InstanceManager.It;

        private static float _lastSlowUpdateTime;
        private static float _nextSlowUpdateTime;
    }
}