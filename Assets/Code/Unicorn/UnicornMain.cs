﻿
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
// using Unicorn.Web;

namespace Unicorn
{
    public sealed class UnicornMain
    {
        static UnicornMain ()
        {
        }

        private UnicornMain ()
        {
        }

        public void Init ()
        {
            if (_isInited)
            {
                return;
            }

            _isInited = true;

			// Default value on Android:
			// Max workerThreads :60  completionPortThreads:30
			// Min workerThreads:4  completionPortThreads:4
//			const int maxWorkerThreads = 8;
//			const int maxCompletionPortThreads = 8;
//			ThreadPool.SetMaxThreads(maxWorkerThreads, maxCompletionPortThreads);

			// force call static ctor of Console class
			Console.Tick();
			_InitLogInfo();

			var persistentDataPath = Application.persistentDataPath;
            var now = DateTime.Now.ToString("yyyy-M-d HH:mm ddd");
			Console.WriteLine("[UnicornMain.Init()]\n{0}\nplatform={1}\nos={2}\ndeviceModel={3}\nprocessorCount={4}\nsystemMemorySize={5}\ngraphicsDevice={6}\ngraphicsMemorySize={7}\n" +
			                  "logPath={8}\ndataPath={9}\nstreamingAssetsPath={10}\npersistentDataPath={11}\nresolution={12}x{13}\nScreen.dpi={14}"
                              , now
			                  , Application.platform.ToString()
			                  , SystemInfo.operatingSystem
			                  , SystemInfo.deviceModel
							  , SystemInfo.processorCount
							  , SystemInfo.systemMemorySize.ToString()
							  , SystemInfo.graphicsDeviceName
			                  , SystemInfo.graphicsMemorySize.ToString()
                              , Constants.LogPath
                              , Application.dataPath
                              , Application.streamingAssetsPath
                              , persistentDataPath
			                  , Screen.width.ToString()
			                  , Screen.height.ToString()
			                  , Screen.dpi.ToString("F2")
			                  );

			if (string.IsNullOrEmpty(persistentDataPath))
			{
				Console.Error.WriteLine("Android Error: persistentDataPath is empty, please restart the Android System.");
			}

            //ParticleRenderQueueRemapper.Enable = true;
			System.Net.ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d)=>true;

            if (null != OnInited)
            {
                OnInited();
            }
        }

//        private bool _AcceptAllCertifications (object sender
//			, System.Security.Cryptography.X509Certificates.X509Certificate certification
//			, System.Security.Cryptography.X509Certificates.X509Chain chain
//			, System.Net.Security.SslPolicyErrors sslPolicyErrors)
//        {
//            return true;
//        }

        private void _InitLogInfo ()
        {
            try 
            {
				var lastLogPath = Constants.LastLogPath;
				var logPath = Constants.LogPath;

                if (File.Exists(logPath))
                {
                    FileTools.Overwrite(logPath, lastLogPath);
                }

                var stream = new FileStream(logPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                _logWriter = new StreamWriter(stream);
                
				Application.logMessageReceived += _HandlerLogCallBack;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[UnicornMain._InitLogInfo()] ex={0}", ex);
            }
        }

        public void Tick (float deltaTime)
        {
            if (_isInited)
            {
				os.frameCount = Time.frameCount;
				os.time = Time.time;

				TickTools.Tick();
				Console.Tick();
                _TickLogs();
                
				CoroutineManager.Tick();
                PartTickSystem.Instance.Tick();
                Loom.Tick();

                //AssetManager.Instance.Tick();
                DisposableRecycler.Tick();
            }
        }

        public void Dispose ()
        {
			if (_isInited)
			{
				if (null != OnDisposing)
				{
					OnDisposing();
				}
				
				if (null != _logWriter)
				{
					_logWriter.Close();
				}
				
//				WebPrefab._GetLruCache().Clear();
//				_coroutineManager.Dispose();
				CoroutineManager.Clear();
				//WebManager.Dispose();
				_isInited = false;
				
				Console.WriteLine("[UnicornMain.Dispose()]");
			}
        }

        private void _HandlerLogCallBack (string logString, string stackTrace, LogType type)
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

//				if (null != OnLogCallBack)
//				{
//					OnLogCallBack(logString, stackTrace, type);
//				}
            }
        }

        private void _TickLogs ()
        {
            var count = _logs.Count;
            if (count > 0 && null != _logWriter)
            {
                for (int i= 0; i< count; ++i)
                {
                    var log = _logs[i];
                    _logWriter.WriteLine(log);
                }

                _logWriter.Flush();
                _logs.Clear();
            }
        }

		public bool IsInited		{ get { return _isInited; } }

//		[System.Obsolete("Application.logMessageReceived")]
//		public event Application.LogCallback OnLogCallBack;
		public event Action			OnInited;
        public event Action   		OnDisposing;

        public static readonly UnicornMain Instance = new ();

        private readonly ArrayList _logs = new ();
        private bool _isInited;
        private StreamWriter _logWriter;
        private string _lastLogString;
    }
}