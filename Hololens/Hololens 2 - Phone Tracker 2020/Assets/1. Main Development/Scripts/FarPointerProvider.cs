using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

public class FarPointerProvider : MonoBehaviour
{
    [SerializeField] string pointerName;
    [ContextMenu("test")]
    
    public IMixedRealityPointer GetPointer()
    {
        foreach (var inputSource in CoreServices.InputSystem.DetectedInputSources)
        {
            foreach (var pointer in inputSource.Pointers)
            {
                Debug.Log(pointerName);
                if (pointer.PointerName == pointerName)
                    return pointer;
            }
        }
        return null;
    }
    public void OnClickEvent()
    {
        var pointer = GetPointer();
        if (pointer == null)
            return;
        MixedRealityInputAction action = new MixedRealityInputAction();
        CoreServices.InputSystem.RaisePointerClicked(pointer, action, 1);
    }
}
