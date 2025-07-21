/********************************************************************
created:    2022-11-14
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

using System;
using System.Collections;
using Unicorn;
using Unicorn.Web;

namespace Clients.Web
{
    public class AssetLoader<T> where T : UnityEngine.Object
    {
        private class AssetItem
        {
            public IWebNode Node; // webItem如果被gc了，则asset也会被回收
            public T Asset;
        }

        public bool LoadAsset(string address, Action<T> handler)
        {
            if (string.IsNullOrEmpty(address))
            {
                return false;
            }

            if (_assetItems.Contains(address))
            {
                return true;
            }

            _assetItems[address] = null; // 加一个哨兵
            var argument = new WebArgument
            {
                key = address
            };

            WebManager.It.LoadAsset(argument, node =>
            {
                if (node.Status == WebStatus.Succeeded)
                {
                    if (node.Asset is T asset)
                    {
                        _assetItems[address] = new AssetItem { Node = node, Asset = asset };
                        Logo.Info("asset={0} is loaded successfully", address);
                        handler?.Invoke(asset);
                        return;
                    }
                }

                _assetItems.Remove(address);
                Logo.Error("failed to load asset={0}", address);
            });

            return true;
        }

        public T GetAsset(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return null;
            }

            var item = _assetItems[address] as AssetItem;
            return item?.Asset;
        }

        // public string GetAnimationClipList()
        // {
        //     string str = "";
        //    foreach(DictionaryEntry clip in _loadedClips) 
        //    {
        //         str += clip.ToString() + ":";
        //    }
        //     return str;
        // }

        private readonly Hashtable _assetItems = new();
    }
}

#endif