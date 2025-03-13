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
            var assetPath = fetus.GetAssetPath();
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
            if (child != null)
            {
                _OnLoadGameObject(fetus, child);
            }
            else if (fetus.GetTransform() is not null)
            {
                _OnLoadGameObject(fetus, fetus.GetTransform());
            }
            else
            {
                _LoadAsset(fetus, assetPath);
            }
        }

        public override void OnExit(WindowFetus fetus, object arg1)
        {
            _loadWindowMask.CloseWindow(fetus.GetAssetPath());
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
                Logo.Warn(
                    $"[LoadState.OnCloseWindow()] _delayedAction={_delayedAction} assetPath={fetus.GetAssetPath()}");
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
                    Logo.Warn(
                        $"[LoadState._OnLoadedPrefab()] _delayedAction={_delayedAction} isLoading={isLoading} assetPath={fetus.GetAssetPath()}");
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
                        _OnLoadGameObject(fetus, goCloned.transform);
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

        private void _OnLoadGameObject(WindowFetus fetus, Transform transform)
        {
            if (fetus.IsDebugging())
            {
                Logo.Warn(
                    $"[LoadState._OnLoadGameObject()] _delayedAction={_delayedAction} assetPath={fetus.GetAssetPath()}");
            }

            fetus.OnLoadGameObject(transform);
            // 因为Loaded这个flag代表gameObject可用性, 因此需要在InnerOnLoaded()事件之前加入
            fetus.AddFlag(FetusFlags.Loaded);

            var master = fetus.master;
            master.InnerOnLoaded("[_OnLoadGameObject()]");

            var next = _delayedAction == DelayedAction.CloseWindow ? StateKind.Unload : StateKind.OpenAnimation;
            _delayedAction = DelayedAction.None;
            fetus.ChangeState(next);
        }

        private DelayedAction _delayedAction; // 遇到了CloseWindow()的请求
    }
}