using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

public class FarPointerProvider : MonoBehaviour
{
    float timeSinceLastClick = 0;
    [SerializeField] string pointerName = "Gaze Pointer";
    public IEnumerable<IMixedRealityPointer> GetPointers()
    {
        // Find gaze pointer, since we only care about gaze for this test
        foreach (var inputSource in CoreServices.InputSystem.DetectedInputSources)
        {
            foreach (var pointer in inputSource.Pointers)
            {
                if (pointer.PointerName == pointerName && pointer.IsActive)
                {
                    yield return pointer;
                    yield break;
                }
            }
        }
        
        //This doesnt seem to work the way I want. It can't find the gaze pointer. I
        /*foreach (var pointer in PointerUtils.GetPointers<IMixedRealityPointer>(Microsoft.MixedReality.Toolkit.Utilities.Handedness.Any))
        {
            if (pointer.IsInteractionEnabled && pointer.IsActive && pointer is GGVPointer)
                yield return pointer;
        }*/
    }
    public void OnClickEvent()
    {
        Debug.Log(Time.time - timeSinceLastClick);
        timeSinceLastClick = Time.time;
        uint id = 0;
        foreach (var pointer in GetPointers())
        {
            var action = new MixedRealityInputAction(id, "onClick");
            CoreServices.InputSystem.RaisePointerClicked(pointer, action, 1);
            id++;
        }
    }
}
