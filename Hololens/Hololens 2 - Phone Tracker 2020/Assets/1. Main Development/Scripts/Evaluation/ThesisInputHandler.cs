using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

public class ThesisInputHandler : MonoBehaviour
{
    public enum InputType { HandPointer, GazePointer }

    [SerializeField] InputType inputType;
    [SerializeField] MixedRealityToolkit mrtk;
    [ContextMenu("Refresh")]
    private void Start()
    {
        SetInputType(inputType);
    }

    public void SetInputType(InputType inputType)
    {
        switch (inputType)
        {
            case InputType.GazePointer:
                PointerUtils.SetGazePointerBehavior(PointerBehavior.Default);
                PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff);
                break;
            case InputType.HandPointer:
                PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);
                PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOn);
                break;
        }
    }
}
