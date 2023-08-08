
/********************************************************************
created:    2017-07-28
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PointerEvent = UnityEngine.Events.UnityEvent<UnityEngine.EventSystems.PointerEventData>;

namespace Unicorn.UI
{
    public class UIButton : Button, IRemoveAllListeners
    {
        void IRemoveAllListeners.RemoveAllListeners()
        {
            onClick.RemoveAllListeners();
            
            _onPointerDown?.RemoveAllListeners();
            _onPointerUp?.RemoveAllListeners();
            _onPointerExit?.RemoveAllListeners();
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
            _onPointerDown?.Invoke(eventData);
        }

        public override void OnPointerUp (PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            _onPointerUp?.Invoke(eventData);
        }

        public override void OnPointerExit (PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _onPointerExit?.Invoke(eventData);
        }

        public PointerEvent onPointerDown
        {
            get { return _onPointerDown ??= new PointerEvent(); }
        }
        
        public PointerEvent onPointerUp
        {
            get { return _onPointerUp ??= new PointerEvent(); }
        }
        
        public PointerEvent onPointerExit
        {
            get { return _onPointerExit ??= new PointerEvent(); }
        }

        private PointerEvent _onPointerDown;
        private PointerEvent _onPointerUp;
        private PointerEvent _onPointerExit;
    }
}