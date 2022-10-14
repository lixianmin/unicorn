
/********************************************************************
created:    2017-07-28
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Unicorn.UI
{
    public class UIButton : Button
    {
        [Serializable]
        public class ButtonLongClickedEvent : UnityEvent
        {
            internal bool IsPressing ()
            {
                return null != _routine;
            }

            internal void StartCoroutine (UIButton button)
            {
                _routine = _CoLongClick();
                button.StartCoroutine(_routine);
            }

            internal void KillCoroutine (UIButton button)
            {
                if (null != _routine)
                {
                    button.StopCoroutine(_routine);
                    _routine = null;
                }
            }

            private IEnumerator _CoLongClick ()
            {
                var dueTime = Time.unscaledTime + holdTime;
                while (Time.unscaledTime < dueTime)
                {
                    yield return null;
                }

                if (IsPressing())
                {
                    _routine = null;
                    Invoke();
                }
            }

            [Tooltip("How long must pointer be down on this object to trigger a long click")]
            public float holdTime = 0.5f;

            private IEnumerator _routine;
        }

        protected override void Awake ()
        {
            base.Awake();

            _SetHasLongClick(hasOnLongClick);
        }

        protected override void OnDestroy()
        {
            onClick.RemoveAllListeners();
            onLongClick?.RemoveAllListeners();
            base.OnDestroy();
        }

        private void _Press ()
        {
            if (!IsActive() || !IsInteractable())
            {
                return;
            }

            onClick.Invoke();
        }

        public override void OnPointerClick (PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            _Press();
        }

        public override void OnSubmit (BaseEventData eventData)
        {
            _Press();

            // if we get set disabled during the press
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable())
            {
                return;
            }

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(_OnFinishSubmit());
        }

        private IEnumerator _OnFinishSubmit ()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }

        public override void OnPointerDown (PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (null != onLongClick && IsPressed())
            {
                onLongClick.StartCoroutine(this);
            }
        }

        public override void OnPointerUp (PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (null != onLongClick && onLongClick.IsPressing())
            {
                onLongClick.KillCoroutine(this);
            }
        }

        public override void OnPointerExit (PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            if (null != onLongClick && onLongClick.IsPressing())
            {
                onLongClick.KillCoroutine(this);
            }
        }

        private void _SetHasLongClick (bool val)
        {
            _hasLongClick = val;

            if (_hasLongClick)
            {
                onLongClick = onLongClick ?? new ButtonLongClickedEvent();
            }
            else
            {
                onLongClick = null;
            }
        }

        public bool hasOnLongClick
        {
            get { return _hasLongClick; }
            set
            {
                if (_hasLongClick != value)
                {
                    _SetHasLongClick(value);
                }
            }
        }

        public ButtonLongClickedEvent onLongClick;
       
        [SerializeField]
        private bool _hasLongClick;
    }
}