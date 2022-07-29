using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickSwapColor : MonoBehaviour, IMixedRealityPointerHandler
{
    private Renderer rend;
    [SerializeField] Material red, green;
    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (rend.sharedMaterial == null || rend.sharedMaterial == green)
            rend.sharedMaterial = red;
        else if (rend.sharedMaterial == red)
            rend.sharedMaterial = green;
        else
            rend.sharedMaterial = red;
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
}
