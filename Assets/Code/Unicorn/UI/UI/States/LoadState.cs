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
    internal class LoadState : UIStateBase
    {
        public override void OnEnter(WindowFetus fetus, object arg1)
        {
            var assetPath = fetus.master.GetAssetPath();
            _loadWindowMask.OpenWindow(assetPath);

            if (string.IsNullOrEmpty(assetPath))
            {
                Logo.Error("assetPath is empty.");
                return;
            }

            var uiRoot = UIManager.It.GetUIRoot();
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
            _loadWindowMask.CloseWindow(fetus.master.GetAssetPath());
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            if (fetus.IsDebugging())
            {
                Logo.Warn("[LoadState.OnOpenWindow()]");
            }

            _delayedAction = DelayedAction.OpenWindow;
        }

        public override void OnCloseWindow(WindowFetus fetus)
        {
            // 如果gameObject已经加载完成了, 则直接转Unload状态
            var isLoaded = fetus.HasFlag(FetusFlags.Loaded);
            if (isLoaded)
            {
                fetus.ChangeState(StateKind.Unload);
            }
            else
            {
                _delayedAction = DelayedAction.CloseWindow;
            }

            if (fetus.IsDebugging())
            {
                Logo.Warn($"[LoadState.OnCloseWindow()] _delayedAction={_delayedAction}");
            }
        }

        private void _LoadAsset(WindowFetus fetus, string assetPath)
        {
            if (fetus.IsDebugging())
            {
                Logo.Warn($"[LoadState._LoadAsset()] _delayedAction={_delayedAction}");
            }

            var argument = new WebArgument { key = assetPath };
            WebManager.It.LoadPrefab(argument, prefab =>
            {
                var node = fetus.GetWebNode();
                node.CopyFrom(prefab);

                var master = fetus.master;
                var isLoading = this == fetus.GetState();

                if (fetus.IsDebugging())
                {
                    Logo.Warn($"[LoadState._OnLoadedPrefab()] _delayedAction={_delayedAction} isLoading={isLoading}");
                }

                if (_delayedAction == DelayedAction.CloseWindow)
                {
                    _delayedAction = DelayedAction.None;
                    fetus.ChangeState(StateKind.None);

                    // master.Dispose(), 会导致当前状态的OnExit()无法被正常调用到, 逻辑闭环就打破了
                    OnExit(fetus, null);
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
                    Logo.Error("invalid state={0}", fetus.GetState());
                }

                prefab.Dispose();
            });
        }

        private void _OnLoadGameObject(WindowFetus fetus, GameObject gameObject)
        {
            if (fetus.IsDebugging())
            {
                Logo.Warn($"[LoadState._OnLoadGameObject()] _delayedAction={_delayedAction}");
            }

            fetus.OnLoadGameObject(gameObject);

            var master = fetus.master;
            master.InnerOnLoaded("[_OnLoadGameObject()]");
            fetus.AddFlag(FetusFlags.Loaded);

            var next = _delayedAction == DelayedAction.CloseWindow ? StateKind.Unload : StateKind.OpenAnimation;
            _delayedAction = DelayedAction.None;
            fetus.ChangeState(next);
        }

        private DelayedAction _delayedAction; // 遇到了CloseWindow()的请求
    }
}