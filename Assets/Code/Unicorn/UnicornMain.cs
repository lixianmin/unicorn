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
using System.Collections;
using System.IO;
using UnityEngine;
using Unicorn.IO;
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
            _InitLogFile();

            var persistentDataPath = Application.persistentDataPath;
            var now = DateTime.Now.ToString("yyyy-M-d HH:mm ddd");
            Logo.Info(
                "[UnicornMain.Init()]\n{0}\nplatform={1}\nos={2}\ndeviceModel={3}\nprocessorCount={4}\nsystemMemorySize={5}\ngraphicsDevice={6}\ngraphicsMemorySize={7}\n" +
                "logPath={8}\ndataPath={9}\nstreamingAssetsPath={10}\npersistentDataPath={11}\nresolution={12}x{13}\nScreen.dpi={14}\nsupportsInstancing={15}\n" +
                "supportsComputeShaders={16}\n"
                , now
                , Application.platform.ToString()
                , SystemInfo.operatingSystem
                , SystemInfo.deviceModel
                , SystemInfo.processorCount
                , SystemInfo.systemMemorySize.ToString()
                , SystemInfo.graphicsDeviceName
                , SystemInfo.graphicsMemorySize.ToString()
                , _GetLogPath()
                , Application.dataPath
                , Application.streamingAssetsPath
                , persistentDataPath
                , Screen.width.ToString()
                , Screen.height.ToString()
                , Screen.dpi.ToString("F2")
                , SystemInfo.supportsInstancing
                , SystemInfo.supportsComputeShaders
            );

            if (string.IsNullOrEmpty(persistentDataPath))
            {
                Logo.Error("Android Error: persistentDataPath is empty, please restart the Android System.");
            }

            System.Net.ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;

            _InsertPlayerLoop();
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

        private static void _InitLogFile()
        {
            try
            {
                var logPath = _GetLogPath();
                if (File.Exists(logPath))
                {
                    var lastLogPath = _GetLastLogPath();
                    FileTools.Overwrite(logPath, lastLogPath);
                }

                var stream = new FileStream(logPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                _logWriter = new StreamWriter(stream);

                Application.logMessageReceived += _HandlerLogCallBack;
            }
            catch (Exception ex)
            {
                Logo.Error("[UnicornMain._InitLogInfo()] ex={0}", ex);
            }
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
            _UpdateLogs();

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
            // _kitManager.SlowUpdate(deltaTime);
            _uiManager.SlowUpdate(deltaTime);
        }

        private static void _HandlerLogCallBack(string logString, string stackTrace, LogType type)
        {
            // the same log should not be write twice.
            if (_lastLogString != logString)
            {
                _lastLogString = logString;

                bool needMark = false;

                if (type == LogType.Error)
                {
                    needMark = true;
                    _logs.Add("[[[Error]]]");
                }
                else if (type == LogType.Exception)
                {
                    needMark = true;
                    _logs.Add("[[[Exception]]]");
                }

                _logs.Add(logString);

                if (type == LogType.Error || type == LogType.Warning || type == LogType.Exception)
                {
                    _logs.Add(stackTrace);
                }

                if (needMark)
                {
                    _logs.Add("[[[-]]]");
                }

                _logs.Add(os.linesep);
            }
        }

        private static void _UpdateLogs()
        {
            var count = _logs.Count;
            if (count > 0 && null != _logWriter)
            {
                for (var i = 0; i < count; ++i)
                {
                    var log = _logs[i];
                    _logWriter.WriteLine(log);
                }

                _logWriter.Flush();
                _logs.Clear();
            }
        }

        /// <summary>
        /// 无论是ice.client还是ice.art项目, persistentDataPath都是: C:/Users/user/AppData/LocalLow/ice/ice_client
        /// </summary>
        /// <returns></returns>
        private static string _GetLogPath()
        {
            if (Application.isEditor)
            {
                return $"{Application.persistentDataPath}/{PathTools.ProjectName}.panda.log";
            }

            return Application.persistentDataPath + "/panda.log";
        }

        private static string _GetLastLogPath()
        {
            if (Application.isEditor)
            {
                return $"{Application.persistentDataPath}/{PathTools.ProjectName}.last_panda.log";
            }

            return Application.persistentDataPath + "/last_panda.log";
        }

        private static readonly ArrayList _logs = new();
        private static readonly PartUpdateSystem _partUpdateSystem = new();

        private static readonly CoroutineManager _coroutineManager = CoroutineManager.It;

        // private static readonly KitManager _kitManager = KitManager.It;
        private static readonly UIManager _uiManager = UIManager.It;
        private static readonly InstanceManager _instanceManager = InstanceManager.It;

        private static StreamWriter _logWriter;
        private static string _lastLogString;

        private static float _lastSlowUpdateTime;
        private static float _nextSlowUpdateTime;
    }
}