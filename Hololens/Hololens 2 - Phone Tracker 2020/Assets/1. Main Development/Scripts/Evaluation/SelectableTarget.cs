using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class SelectableTarget : MonoBehaviour, IMixedRealityPointerHandler, IMixedRealityFocusHandler
{
    [SerializeField] float scaleAmount = 1.5f, scaleTime = 0.2f;
    [SerializeField] UnityEvent onSelected, onFocusEnter, onFocusExit;
    Vector3 ogScale;
    Sequence scaleSequence;

    private void Awake()
    {
        ogScale = transform.localScale;
    }
    #region Pointer
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (scaleSequence != null)
            scaleSequence.Complete();
        scaleSequence = DOTween.Sequence();
        var scaleTime = this.scaleTime * 0.5f;
        var scaleUp = transform.DOScale(ogScale * scaleAmount, scaleTime);
        var scaleDown = transform.DOScale(ogScale, scaleTime);
        scaleSequence.Append(scaleUp).Append(scaleDown);
        scaleSequence.onComplete = () => scaleSequence = null;

        onSelected?.Invoke();
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
    public void OnFocusEnter(FocusEventData eventData)
    {
        onFocusEnter?.Invoke();
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        onFocusExit?.Invoke();
    }
    #endregion
}
