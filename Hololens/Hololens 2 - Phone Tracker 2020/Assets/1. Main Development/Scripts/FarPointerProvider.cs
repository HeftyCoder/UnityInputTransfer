using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

public class FarPointerProvider : MonoBehaviour
{
    [SerializeField] string pointerName;
    float timeSinceLastClick = 0;
    
    public IMixedRealityPointer GetPointer()
    {
        foreach (var inputSource in CoreServices.InputSystem.DetectedInputSources)
        {
            foreach (var pointer in inputSource.Pointers)
            {
                if (pointer.PointerName == pointerName && pointer.IsInteractionEnabled && pointer.IsActive)
                    return pointer;
            }
        }
        return null;
    }
    public void OnClickEvent()
    {
        Debug.Log(Time.time - timeSinceLastClick);
        timeSinceLastClick = Time.time;
        var pointer = GetPointer();
        if (pointer == null)
            return;
        MixedRealityInputAction action = new MixedRealityInputAction();
        CoreServices.InputSystem.RaisePointerClicked(pointer, action, 1);
    }
}
