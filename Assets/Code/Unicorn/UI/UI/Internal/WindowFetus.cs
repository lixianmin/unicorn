/********************************************************************
created:    2022-08-15
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.UI.States;
using Unicorn.Web;
using UnityEngine;

namespace Unicorn.UI.Internal
{
    internal class WindowFetus
    {
        public WindowFetus(UIWindowBase master)
        {
            this.master = master;
        }

        public void Dispose()
        {
            CloseWindow();

            _parent = null;
            _serializer = null;
            _webNode.Reset();

            // if (_transform is not null)
            if (_transform != null)
            {
                // 如果是3D界面，则不销毁
                if (master._is2D)
                {
                    Object.Destroy(_transform.gameObject);
                }

                _transform = null;
            }
        }

        /// <summary>
        /// 因为改成使用ExpensiveUpdate()实现状态转换，现在可以在OnEnter()与OnExit()中调用了
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="arg1"></param>
        public void ChangeState(StateKind kind, object arg1 = null)
        {
            _nextKind = kind;
            _nextArg1 = arg1;

            if (IsDebugging())
            {
                Logo.Warn($"[ChangeState()] _nextKind={_nextKind}, assetPath={master.GetAssetPath()}");
            }
        }

        public void ExpensiveUpdate()
        {
            if (_lastKind != _nextKind)
            {
                if (IsDebugging())
                {
                    var assetPath = master.GetAssetPath();
                    Logo.Warn($"[ExpensiveUpdate()] {_lastKind}=>{_nextKind}, _state={_state}, assetPath={assetPath}");
                }

                // ~~在OnExit()/OnEnter()的过程中, _nextKind有可能会改~~
                _lastKind = _nextKind;

                _state.OnExit(this, _nextArg1);
                _state = UIStateBase.Create(_nextKind);
                _state.OnEnter(this, _nextArg1);
            }
        }

        public void OnLoadGameObject(GameObject gameObject)
        {
            _transform = gameObject.transform;
            var serializer = gameObject.GetComponent(typeof(UISerializer)) as UISerializer;
            // 接下来，计划无论有无UISerializer脚本，UI相关代码都可以正常运行
            // if (serializer is null)
            // {
            //     Logo.Error("serializer is null, gameObject={0}", goCloned.ToString());
            // }

            _serializer = serializer;

            if (_parent is not null)
            {
                _transform.SetParent(_parent, false);
            }

            // 对于UI来说，canvas其实是必须的
            var canvas = gameObject.GetComponent(typeof(Canvas)) as Canvas;
            if (canvas != null)
            {
                canvas.overrideSorting = true;

                var is2D = canvas.renderMode != RenderMode.WorldSpace;
                master._is2D = is2D;

                // 2d和3d界面分别设置不同的camera
                if (is2D)
                {
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = UIManager.It.GetUICamera();
                }
                else
                {
                    canvas.worldCamera = Camera.main;
                }

                // 自动设置layer：如果后续有不自动调整layer的需求，只需要在UISerializer中补一个autoSetLayer变量控制即可
                var layerName = is2D ? "UI" : "Default";
                var layer = LayerMask.NameToLayer(layerName);
                if (gameObject.layer != layer)
                {
                    gameObject.SetLayerRecursively(layer);
                }
            }
            else
            {
                // 当不存在canvas的时候, GetComponent()好像也能给取一个出来, 只是使用is not null判断会失败
                canvas = null;
            }

            master._InitComponents(_transform, canvas);
            master._InitWidgetsWindow();
            master._FillWidgets(serializer);
            UIManager.It._ActivateWindow(master);
        }

        public void OpenWindow()
        {
            _state.OnOpenWindow(this);

            var flags = (FetusFlags)master.GetWindowFlags();
            AddFlag(flags);
            AddFlag(FetusFlags.Opened1);
        }

        public void CloseWindow()
        {
            RemoveFlag(FetusFlags.Opened1);
            _state.OnCloseWindow(this);
        }

        public void SetActive(bool isActive)
        {
            if (_transform is not null)
            {
                var go = _transform.gameObject;
                if (null != go && go.activeSelf != isActive)
                {
                    go.SetActive(isActive);
                }
            }
        }

        internal UIStateBase GetState()
        {
            return _state;
        }

        public UISerializer GetSerializer()
        {
            return _serializer;
        }

        /// <summary>
        /// 用于度量window的加载进度
        /// </summary>
        /// <returns></returns>
        public WebData GetWebNode()
        {
            return _webNode;
        }

        public void AddFlag(FetusFlags flag)
        {
            _flags |= flag;
        }

        public void RemoveFlag(FetusFlags flag)
        {
            _flags &= ~flag;
        }

        public bool HasFlag(FetusFlags flag)
        {
            return (_flags & flag) != 0;
        }

        internal bool IsDebugging()
        {
            // return master.GetAssetPath().EndsWith(" for top-up.prefab");
            return false;
        }

        internal readonly UIWindowBase master;

        private UIStateBase _state = UIStateBase.Create(StateKind.None);
        private StateKind _lastKind = StateKind.None;
        private StateKind _nextKind = StateKind.None;
        private object _nextArg1;

        private Transform _transform;
        private Transform _parent = UIManager.It.GetUIRoot();
        private UISerializer _serializer;
        private readonly WebData _webNode = new();
        private FetusFlags _flags;
    }
}