/********************************************************************
created:    2017-07-28
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
using Unicorn.Web;
using UnityEngine;
using UnityEngine.UI;

namespace Unicorn.UI
{
    public class UIRawImage : RawImage
    {
        public IWebNode LoadImage(string imagePath)
        {
            const string method = nameof(LoadImage);
            if (string.IsNullOrEmpty(imagePath))
            {
                Logo.Warn($"[{method}] imagePath is empty");
                return EmptyWebNode.It;
            }

            if (imagePath == _imagePath)
            {
                Logo.Info($"[{method}] load the same image, imagePath={imagePath}");
                return EmptyWebNode.It;
            }

            return imagePath.StartsWith("Assets") ? _LoadTexture(imagePath) : _DownloadImage(imagePath);
        }

        private IWebNode _LoadTexture(string imagePath)
        {
            const string method = nameof(_LoadTexture);

            var version = ++_version;
            var argument = new WebArgument { key = imagePath };
            return WebManager.It.LoadAsset(argument, node =>
            {
                if (!_CheckAcceptImage(method, node, imagePath, version))
                {
                    return;
                }

                if (node.Asset is not Texture2D asset)
                {
                    Logo.Warn($"[{method}] asset is not Texture2D, imagePath={imagePath}");
                    return;
                }

                _web?.Dispose();
                _web = node as IDisposable;
                _imagePath = imagePath;

                texture = asset;
                Logo.Info($"[{method}] load texture success, imagePath={imagePath}");
            });
        }

        private HttpTexture _DownloadImage(string imagePath)
        {
            const string method = nameof(_DownloadImage);
            var version = ++_version;
            return new HttpTexture(imagePath, node =>
            {
                if (!_CheckAcceptImage(method, node, imagePath, version))
                {
                    return;
                }

                _web?.Dispose();
                _web = node;
                _imagePath = imagePath;

                texture = node.GetTexture();
                Logo.Info($"[{method}] download image success, imagePath={imagePath}");
            });
        }

        private bool _CheckAcceptImage(string method, IWebNode node, string imagePath, int version)
        {
            if (node.Status != WebStatus.Succeeded)
            {
                Logo.Info($"[{method}] failed to download, imagePath={imagePath}");
                return false;
            }

            if (version != _version)
            {
                Logo.Info($"[{method}] version has changed, imagePath={imagePath}");
                return false;
            }

            if (this == null)
            {
                Logo.Info($"[{method}] rawImage has been destroyed, imagePath={imagePath}");
                return false;
            }

            return true;
        }

        // _web和_imagePath需要同步修改
        private IDisposable _web;
        private string _imagePath = string.Empty;

        // _version是提前修改的
        private int _version;
    }
}