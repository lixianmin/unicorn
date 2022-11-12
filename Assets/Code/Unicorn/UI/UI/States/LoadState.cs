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
            AssertTools.IsTrue(!fetus.isDelayedCloseWindow);
            _loadWindowMask.OpenWindow();

            var assetPath = fetus.master.GetAssetPath();
            if (string.IsNullOrEmpty(assetPath))
            {
                Console.Error.WriteLine("assetPath is empty.");
                return;
            }

            // 2d界面通常是单独加载的，而3d界面则可以随场景一起加载（此时直接用uibag这样的名字查找到对应的gameObject）
            var needLoadAsset = fetus.master.Is2D();
            if (needLoadAsset)
            {
                _LoadAsset(fetus, assetPath);
            }
            else
            {
                _FindGameObject(fetus, assetPath);
            }
        }
        
        public override void OnExit(WindowFetus fetus, object arg1)
        {
            _loadWindowMask.CloseWindow();
            AssertTools.IsTrue(!fetus.isDelayedCloseWindow);
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            fetus.isDelayedCloseWindow = false;
        }

        public override void OnCloseWindow(WindowFetus fetus)
        {
            fetus.isDelayedCloseWindow = true;
        }

        private void _LoadAsset(WindowFetus fetus, string assetPath)
        {
            var argument = new WebArgument { key = assetPath };
            WebManager.Instance.LoadPrefab(argument, prefab =>
            {
                var master = fetus.master;
                var isLoading = this == fetus.GetState();
                if (fetus.isDelayedCloseWindow)
                {
                    fetus.isDelayedCloseWindow = false;
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

        private void _FindGameObject(WindowFetus fetus, string name)
        {
            var master = fetus.master;
            var uiRoot = UIManager.Instance.GetUIRoot();
            var child = uiRoot.Find(name);
            if (null == child)
            {
                Console.Error.WriteLine($"can not find transform under UIRoot in scene, assetPath={name}");
                return;
            }
            
            var gameObject = child.gameObject;
            fetus.OnLoadGameObject(gameObject);
            CallbackTools.Handle(master.InnerOnLoaded, "[_FindGameObject()]");
            fetus.isLoaded = true;
            fetus.ChangeState(StateKind.OpenAnimation);
        }
    }
}