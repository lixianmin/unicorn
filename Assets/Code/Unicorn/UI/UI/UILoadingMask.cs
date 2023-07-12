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

        public void OpenWindow()
        {
            _EnsureCreateGameObject();

            if (_openCount == 0)
            {
                _gameObject.SetActive(true);
                _image.color = new Color(0, 0, 0, 0);
                
                _transform.SetAsLastSibling();
                if (_discolorTime > 0)
                {
                    _discolorRoutine = _CoDelayedDiscolor();
                    CoroutineManager.It.StartCoroutine(_discolorRoutine);
                }
            }

            _openCount++;
        }

        public void CloseWindow()
        {
            if (_openCount <= 0)
            {
                return;
            }

            _openCount--;
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
            
            var goLoadingMask = new GameObject("LoadingMask");
            var image = goLoadingMask.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0);

            const int bigEnoughScale = 500;
            var transform = goLoadingMask.transform;
            transform.localScale = new Vector3(bigEnoughScale, bigEnoughScale, bigEnoughScale);

            var parent = UIManager.It.GetUIRoot();
            transform.SetParent(parent, false);

            _gameObject = goLoadingMask;
            _transform = transform;
            _image = image;
        }

        private GameObject _gameObject;
        private Transform _transform;
        private Image _image;

        private IEnumerator _discolorRoutine;
        private float _discolorTime;
        private int _openCount;
    }
}