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
            AssertTools.IsTrue(!_isDelayedCloseWindow);
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
                _UsePreloadGameObject(fetus, child.gameObject);
            }
        }
        
        public override void OnExit(WindowFetus fetus, object arg1)
        {
            _loadWindowMask.CloseWindow();
            AssertTools.IsTrue(!_isDelayedCloseWindow);
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            _isDelayedCloseWindow = false;
        }

        public override void OnCloseWindow(WindowFetus fetus)
        {
            _isDelayedCloseWindow = true;
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
                if (_isDelayedCloseWindow)
                {
                    _isDelayedCloseWindow = false;
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
                        fetus.OnLoadGameObject(goCloned);
                        CallbackTools.Handle(master.InnerOnLoaded, "[_LoadAsset()]");
                        fetus.isLoaded = true;
                        fetus.ChangeState(StateKind.OpenAnimation);
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
        
        private void _UsePreloadGameObject(WindowFetus fetus, GameObject gameObject)
        {
            fetus.OnLoadGameObject(gameObject);
            
            var master = fetus.master;
            CallbackTools.Handle(master.InnerOnLoaded, "[_UsePreloadGameObject()]");
            fetus.isLoaded = true;
            fetus.ChangeState(StateKind.OpenAnimation);
        }
        
        private bool _isDelayedCloseWindow; // 遇到了CloseWindow()的请示
    }
}