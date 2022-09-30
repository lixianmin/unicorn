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

            if (_transform is not null)
            {
                Object.Destroy(_transform.gameObject);
                _transform = null;
            }
        }
        
        public void ChangeState(StateKind kind, object arg1=null)
        {
            _state?.OnExit(this, arg1);
            _state = StateBase.Create(kind);
            _state?.OnEnter(this, arg1);
        }

        public void OnLoadGameObject(GameObject goCloned)
        {
            _transform = goCloned.transform;
            var serializer = goCloned.GetComponent(typeof(UISerializer)) as UISerializer;
            if (serializer is null)
            {
                Console.Error.WriteLine("serializer is null, gameObject={0}", goCloned.ToString());
            }

            _serializer = serializer;
            
            if (_parent is not null)
            {
                _transform.SetParent(_parent, false);
            }

            var canvas = goCloned.GetComponent(typeof(Canvas)) as Canvas;
            if (canvas != null)
            {
                canvas.overrideSorting = true;
            }
            else
            {   // 当不存在canvas的时候, GetComponent()好像也能给取一个出来, 只是使用is not null判断会失败
                canvas = null;
            }
            
            master._InitComponents(_transform, canvas);
            master._InitWidgetsWindow();
            master._FillWidgets(serializer);
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

        public readonly UIWindowBase master;

        private StateBase _state = StateBase.Create(StateKind.None);
        private Transform _transform;
        private Transform _parent = UIManager.GetUIRoot();
        private UISerializer _serializer;
        
        public float activateTime = 0;
        public bool isWindowCached;
        public bool isLoaded;
        public bool isOpened;
        public bool isDelayedOpenWindow;
        public bool isDelayedCloseWindow;
    }
}