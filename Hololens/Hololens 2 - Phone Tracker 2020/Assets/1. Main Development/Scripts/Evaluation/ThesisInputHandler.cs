using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class ThesisInputHandler : MonoBehaviour
{
    public enum InputType { HandPointer, GazePointer }

    [SerializeField] InputType inputType;

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
                PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOn);
                PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff);
                break;
            case InputType.HandPointer:
                PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);
                PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOn);
                break;
        }
    }
}
