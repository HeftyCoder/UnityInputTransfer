using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

public class FarPointerProvider : MonoBehaviour
{
    float timeSinceLastClick = 0;
    
    public IEnumerable<IMixedRealityPointer> GetPointers()
    {
        foreach (var inputSource in CoreServices.InputSystem.DetectedInputSources)
        {
            foreach (var pointer in inputSource.Pointers)
            {
                if (pointer.IsInteractionEnabled && pointer.IsActive)
                    yield return pointer;
            }
        }
    }
    public void OnClickEvent()
    {
        Debug.Log(Time.time - timeSinceLastClick);
        timeSinceLastClick = Time.time;
        MixedRealityInputAction action = new MixedRealityInputAction();
        foreach (var pointer in GetPointers())
            CoreServices.InputSystem.RaisePointerClicked(pointer, action, 1);
    }
}
