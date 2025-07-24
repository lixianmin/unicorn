/********************************************************************
created:    2024-02-22
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

using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Unicorn.IO;
using Object = UnityEngine.Object;

namespace Unicorn.Web
{
    public class HttpTexture : Disposable, IWebNode
    {
        public HttpTexture(string url, Action<HttpTexture> fn = null)
        {
            if (url.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(url));
            }

            _url = url;
            Status = WebStatus.Loading;

            // 初始化缓存路径
            _InitCacheDirectory();
            _cacheFilePath = _GetCacheFilePath(url);

            if (File.Exists(_cacheFilePath))
            {
                CoroutineManager.It.StartCoroutine(_CoCheckLoadOrDownload(url, fn));
            }
            else
            {
                CoroutineManager.It.StartCoroutine(_CoDownloadImage(url, fn));
            }

            AtDisposing += _AtDisposing;
        }

        private IEnumerator _CoCheckLoadOrDownload(string url, Action<HttpTexture> fn)
        {
            // 1. 在 try...catch 外部启动异步文件读取任务
            // 这行代码本身不会抛出文件相关的异常，它只是创建并启动一个任务
            var readTask = File.ReadAllBytesAsync(_cacheFilePath);

            // 2. 等待任务完成。这个循环现在位于 try...catch 之外，是合法的。
            // 协程会在这里暂停，直到文件读取操作结束（无论成功或失败）。
            while (!readTask.IsCompleted)
            {
                yield return null;
            }

            try
            {
                // 3. 在 try 块内部获取任务的结果。
                // 如果任务因异常（如文件未找到、无权限）而失败，
                // 访问 .Result 会将该异常抛出，并被下面的 catch 块捕获。
                var fileData = readTask.Result;
                
                // 创建最小的占位纹理，LoadImage会自动调整为实际大小
                _texture = new Texture2D(1, 1);
                if (_texture.LoadImage(fileData))
                {
                    Status = WebStatus.Succeeded;
                    CallbackTools.Handle(ref fn, this);
                    // Logo.Info($"[_CoCheckLoadOrDownload] loaded texture done, _cacheFilePath={_cacheFilePath}");
                    yield break;
                }
            }
            catch (Exception ex)
            {
                Logo.Warn($"[_CheckLoadFromCache] Failed to load cached texture from {_cacheFilePath}: {ex.Message}");
            }

            Object.Destroy(_texture);
            _texture = null;

            // 缓存文件可能损坏，删除并重新下载
            _DeleteCacheFileSafely(_cacheFilePath);

            // 如果本机加载失败, 则启动网络下载
            var iter = _CoDownloadImage(url, fn);
            while (iter.MoveNext())
            {
                yield return null;
            }
        }

        private static void _InitCacheDirectory()
        {
            if (_isCacheInitialized)
            {
                return;
            }

            _cacheDirectory = Path.Combine(Application.persistentDataPath, "http_texture_cache");
            if (!Directory.Exists(_cacheDirectory))
            {
                Directory.CreateDirectory(_cacheDirectory);
            }

            _isCacheInitialized = true;
        }

        private static string _GetCacheFilePath(string url)
        {
            // 使用URL的MD5作为文件名，避免特殊字符问题
            var urlBytes = Encoding.UTF8.GetBytes(url);
            var hash = Md5sum.It.GetHexDigest32(urlBytes);

            // 添加.jpg扩展名，便于识别
            return Path.Combine(_cacheDirectory, hash + ".jpg");
        }

        private IEnumerator _CoDownloadImage(string url, Action<HttpTexture> fn)
        {
            var request = UnityWebRequestTexture.GetTexture(url);
            request.SendWebRequest();
            while (!request.isDone)
            {
                yield return null;
            }

            Status = request.result == UnityWebRequest.Result.Success ? WebStatus.Succeeded : WebStatus.Failed;
            if (Status == WebStatus.Succeeded)
            {
                _texture = DownloadHandlerTexture.GetContent(request);

                // 保存到缓存
                var data = request.downloadHandler.data;
                try
                {
                    if (data is { Length: > 0 })
                    {
                        FileTools.WriteAllBytesSafely(_cacheFilePath, data);
                    }
                }
                catch (Exception ex)
                {
                    Logo.Warn($"[_SaveToCache] Failed to save texture to cache {_cacheFilePath}: {ex.Message}");
                }
            }

            request.Dispose();
            CallbackTools.Handle(ref fn, this);
        }

        private static void _DeleteCacheFileSafely(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                Logo.Warn($"[_DeleteCacheFileSafely] Failed to delete cache file {filePath}: {ex.Message}");
            }
        }

        private void _AtDisposing()
        {
            if (_texture != null)
            {
                Object.Destroy(_texture);
                _texture = null;
            }

            Status = WebStatus.None;
        }

        public Texture2D GetTexture()
        {
            return _texture;
        }

        public string GetUrl()
        {
            return _url;
        }

        public void Cancel()
        {
        }

        /// <summary>
        /// 清理所有缓存文件
        /// </summary>
        public static void ClearAllCache()
        {
            _InitCacheDirectory();

            try
            {
                if (Directory.Exists(_cacheDirectory))
                {
                    var files = Directory.GetFiles(_cacheDirectory);
                    foreach (var file in files)
                    {
                        _DeleteCacheFileSafely(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Logo.Warn($"[ClearAllCache] Failed to clear cache directory: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取缓存大小（字节）
        /// </summary>
        public static long GetCacheSize()
        {
            _InitCacheDirectory();

            long totalSize = 0;
            try
            {
                if (Directory.Exists(_cacheDirectory))
                {
                    var files = Directory.GetFiles(_cacheDirectory);
                    foreach (var file in files)
                    {
                        var fileInfo = new FileInfo(file);
                        totalSize += fileInfo.Length;
                    }
                }
            }
            catch (Exception ex)
            {
                Logo.Warn($"[GetCacheSize] Failed to calculate cache size: {ex.Message}");
            }

            return totalSize;
        }

        public WebStatus Status { get; private set; }
        public Object Asset => _texture;

        private Texture2D _texture;
        private readonly string _url;
        private readonly string _cacheFilePath;

        // 静态缓存配置
        private static string _cacheDirectory;
        private static bool _isCacheInitialized;
    }
}