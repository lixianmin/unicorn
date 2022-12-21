/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.UI.Internal;
using Unicorn.Web;
using UnityEngine;

namespace Unicorn.UI.States
{
    internal class LoadState : StateBase
    {
        public override void OnEnter(WindowFetus fetus, object arg1)
        {
            _loadWindowMask.OpenWindow();

            var assetPath = fetus.master.GetAssetPath();
            if (string.IsNullOrEmpty(assetPath))
            {
                Console.Error.WriteLine("assetPath is empty.");
                return;
            }

            var uiRoot = UIManager.Instance.GetUIRoot();
            var child = uiRoot.Find(assetPath);
            
            // 如果在UIRoot下找到名为assetPath的节点，则直接使用该节点；否则当作UI资源的路径从addressable加载
            // 通常3d界面可能是内置到场景中的
            var needLoadAsset = child == null;
            if (needLoadAsset)
            {
                _LoadAsset(fetus, assetPath);
            }
            else
            {
                _OnLoadGameObject(fetus, child.gameObject);
            }
        }
        
        public override void OnExit(WindowFetus fetus, object arg1)
        {
            _loadWindowMask.CloseWindow();
            AssertTools.IsTrue(!_isDelayedClosing);
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            _isDelayedClosing = false;
        }

        public override void OnCloseWindow(WindowFetus fetus)
        {
            _isDelayedClosing = true;
        }

        private void _LoadAsset(WindowFetus fetus, string assetPath)
        {
            var argument = new WebArgument { key = assetPath };
            WebManager.Instance.LoadPrefab(argument, prefab =>
            {
                var node = fetus.GetWebNode();
                node.CopyProperty(prefab);
                
                var master = fetus.master;
                var isLoading = this == fetus.GetState();
                if (_isDelayedClosing)
                {
                    _isDelayedClosing = false;
                    fetus.ChangeState(StateKind.None);
                    master.Dispose();
                }
                else if (isLoading)
                {
                    var mainAsset = prefab.Asset;
                    var goCloned = Object.Instantiate(mainAsset);
                    
                    if (goCloned is not null)
                    {
                        goCloned.name = mainAsset.name;
                        _OnLoadGameObject(fetus, goCloned);
                    }
                    else
                    {
                        fetus.ChangeState(StateKind.Failure, prefab.ToString());
                    }
                }
                else
                {
                    Console.Error.WriteLine("invalid state={0}", fetus.GetState());
                }

                prefab.Dispose();
            });
        }
        
        private void _OnLoadGameObject(WindowFetus fetus, GameObject gameObject)
        {
            fetus.OnLoadGameObject(gameObject);

            var master = fetus.master;
            master.InnerOnLoaded( "[_OnLoadGameObject()]");
            fetus.isLoaded = true;

            var next = _isDelayedClosing ? StateKind.Unload : StateKind.OpenAnimation;
            _isDelayedClosing = false;
            fetus.ChangeState(next);
        }
        
        private bool _isDelayedClosing; // 遇到了CloseWindow()的请示
    }
}