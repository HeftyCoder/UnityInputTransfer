using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System;

namespace UOPHololens.Evaluation
{
    [RequireComponent(typeof(PointerHandler), typeof(FocusHandler))]
    public class SelectableTarget : MonoBehaviour, IMixedRealityPointerHandler, IMixedRealityFocusHandler
    {
        [SerializeField] float scaleAmount = 1.5f, scaleTime = 0.2f;
        [SerializeField] Material enabledMaterial, disabledMaterial;

        PointerHandler pointerHandler;
        FocusHandler focusHandler;
        public event Action<SelectableTarget> onClicked;
        public event Action<SelectableTarget> onFocusEnter;
        public event Action<SelectableTarget> onFocusExit;

        Vector3 ogScale;
        Sequence scaleSequence;
        Renderer rend;
        private void Awake()
        {
            ogScale = transform.localScale;
            rend = GetComponent<Renderer>();
            pointerHandler = GetComponent<PointerHandler>();
            focusHandler = GetComponent<FocusHandler>();
        }
        private void OnEnable()
        {
            foreach (var pointer in PointerUtils.GetPointers())
            {
                var go = pointer?.Result?.CurrentPointerTarget;
                if (go == gameObject)
                {
                    focusHandler.OnFocusEnterEvent?.Invoke();
                    break;
                }
            }
            pointerHandler.enabled = true;
            focusHandler.enabled = true;
            rend.sharedMaterial = enabledMaterial;
        }
        private void OnDisable()
        {
            focusHandler.OnFocusExitEvent?.Invoke();
            rend.sharedMaterial = disabledMaterial;
            pointerHandler.enabled = false;
            focusHandler.enabled = false;
        }
        #region Pointer
        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            OnClick();
        }
        private void OnClick()
        {
            if (scaleSequence != null)
                scaleSequence.Complete();
            scaleSequence = DOTween.Sequence();
            var scaleTime = this.scaleTime * 0.5f;
            var scaleUp = transform.DOScale(ogScale * scaleAmount, scaleTime);
            var scaleDown = transform.DOScale(ogScale, scaleTime);
            scaleSequence.Append(scaleUp).Append(scaleDown);
            scaleSequence.onComplete = () => scaleSequence = null;

            onClicked?.Invoke(this);
        }

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {

        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {

        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {

        }
        #endregion

        #region Focus
        public void OnFocusEnter(FocusEventData eventData) => onFocusEnter?.Invoke(this);

        public void OnFocusExit(FocusEventData eventData) => onFocusExit?.Invoke(this);
        #endregion

    }
}