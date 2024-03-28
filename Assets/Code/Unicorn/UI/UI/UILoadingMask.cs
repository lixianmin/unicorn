/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Unicorn.UI
{
    internal class UILoadingMask
    {
        public UILoadingMask(float discolorTime)
        {
            _discolorTime = Mathf.Max(discolorTime, 0);
        }

        public void OpenWindow(string assetPath)
        {
            // Logo.Warn($"[OpenWindow()] _openCount={_openCount}, assetPath={assetPath}");
            _EnsureCreateGameObject();

            if (_openCount == 0)
            {
                _gameObject.SetActive(true);
                _image.color = new Color(0, 0, 0, 0);

                if (_discolorTime > 0)
                {
                    _discolorRoutine = _CoDelayedDiscolor();
                    CoroutineManager.It.StartCoroutine(_discolorRoutine);
                }
            }

            _openCount++;
        }

        public void CloseWindow(string assetPath)
        {
            if (_openCount <= 0)
            {
                // Logo.Warn($"[CloseWindow()] _openCount={_openCount}, assetPath={assetPath}");
                return;
            }

            _openCount--;
            // Logo.Warn($"[CloseWindow()] _openCount={_openCount}, assetPath={assetPath}");
            if (_openCount == 0)
            {
                _gameObject.SetActive(false);
                if (_discolorRoutine != null)
                {
                    CoroutineManager.It.KillCoroutine(ref _discolorRoutine);
                }
            }
        }

        // Firstly the LoadingMask is transparent, but if the loading time is too long, its color will turn into gray
        private IEnumerator _CoDelayedDiscolor()
        {
            var endTime = Time.time + _discolorTime;
            while (Time.time < endTime)
            {
                yield return null;
            }

            _image.color = new Color(0, 0, 0, 0.506f);
            _discolorRoutine = null;
        }

        private void _EnsureCreateGameObject()
        {
            if (_gameObject is not null)
            {
                return;
            }

            var go = new GameObject("loading_mask");
            _AddCanvas(go);
            _AddImage(go);

            _gameObject = go;
        }

        private void _AddCanvas(GameObject go)
        {
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // 这个是能否接收到mouse点击的关键
            go.AddComponent<GraphicRaycaster>();

            // 设置父节点为UIRoot
            var parent = UIManager.It.GetUIRoot();
            go.transform.SetParent(parent, false);
        }

        private void _AddImage(GameObject go)
        {
            var image = go.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0);
            image.raycastTarget = true;

            _image = image;
        }

        private GameObject _gameObject;
        private Image _image;

        private IEnumerator _discolorRoutine;
        private readonly float _discolorTime;
        private int _openCount;
    }
}