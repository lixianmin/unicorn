/********************************************************************
created:    2022-11-14
author:     lixianmin

Copyright (C) - All Rights Reserved
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