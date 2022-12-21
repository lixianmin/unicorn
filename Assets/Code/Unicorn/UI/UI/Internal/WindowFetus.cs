/********************************************************************
created:    2022-08-15
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/


using Unicorn.UI.States;
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

            if (_transform is not null)
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
        /// 这个方法可以在OnEnter()中调用，但不适合在OnExit()方法调用，因为_state会在OnExit()后被替换掉
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="arg1"></param>
        public void ChangeState(StateKind kind, object arg1 = null)
        {
            _state?.OnExit(this, arg1);
            _state = StateBase.Create(kind);
            _state?.OnEnter(this, arg1);
        }

        public void OnLoadGameObject(GameObject gameObject)
        {
            _transform = gameObject.transform;
            var serializer = gameObject.GetComponent(typeof(UISerializer)) as UISerializer;
            // 接下来，计划无论有无UISerializer脚本，UI相关代码都可以正常运行
            // if (serializer is null)
            // {
            //     Console.Error.WriteLine("serializer is null, gameObject={0}", goCloned.ToString());
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
                    canvas.worldCamera = UIManager.Instance.GetUICamera();
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
                    gameObject.SetLayerRecursivelyEx(layer);
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
            UIManager.Instance._ActivateWindow(master);
        }

        public void OpenWindow()
        {
            _state.OnOpenWindow(this);
        }

        public void CloseWindow()
        {
            _state.OnCloseWindow(this);
        }

        public void SetActive(bool isActive)
        {
            if (_transform is not null)
            {
                _transform.gameObject.SetActiveEx(isActive);
            }
        }

        public StateBase GetState()
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
        public WebNode GetWebNode()
        {
            return _webNode;
        }

        internal readonly UIWindowBase master;

        private StateBase _state = StateBase.Create(StateKind.None);
        private Transform _transform;
        private Transform _parent = UIManager.Instance.GetUIRoot();
        private UISerializer _serializer;
        private readonly WebNode _webNode = new();

        public bool isWindowCached;
        public bool isLoaded;
        public bool isOpened;
    }
}