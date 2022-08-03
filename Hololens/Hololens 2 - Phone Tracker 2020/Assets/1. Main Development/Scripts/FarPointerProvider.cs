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
        foreach (var pointer in PointerUtils.GetPointers<IMixedRealityPointer>(Microsoft.MixedReality.Toolkit.Utilities.Handedness.Any))
        {
            if (pointer.IsInteractionEnabled && pointer.IsActive)
                yield return pointer;
        }
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
