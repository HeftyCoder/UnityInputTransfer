using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Layouts;
#if UNITY_EDITOR
using UnityEditor;
#endif
public struct TransformFollowerDeviceState : IInputStateTypeInfo
{
    public FourCC format => new FourCC('T', 'R', 'A', 'F');

    [InputControl(name = "position", layout = "Vector3", displayName = "Position")]
    public Vector3 position;
    [InputControl(name = "rotation", layout = "Quaternion", displayName = "Rotation")]
    public Quaternion rotation;
}

#if UNITY_EDITOR
[InitializeOnLoad] // Call static class constructor in editor.
#endif
[InputControlLayout(stateType = typeof(TransformFollowerDeviceState))]
public class TransformFollowerDevice : InputDevice
{
#if UNITY_EDITOR
    static TransformFollowerDevice()
    {
        // Trigger our RegisterLayout code in the editor.
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        InputSystem.RegisterLayout<TransformFollowerDevice>(
            matches: new InputDeviceMatcher()
                .WithInterface("Custom"));
    }
    public static TransformFollowerDevice current { get; private set; }
    public Vector3Control devicePosition { get; private set; }
    public QuaternionControl deviceRotation { get; private set; }


    protected override void FinishSetup()
    {
        base.FinishSetup();
        devicePosition = GetChildControl<Vector3Control>("position");
        deviceRotation = GetChildControl<QuaternionControl>("rotation");
    }

    public override void MakeCurrent()
    {
        base.MakeCurrent();
        current = this;
    }
    protected override void OnRemoved()
    {
        base.OnRemoved();
        if (current == this)
            current = null;
    }
}

