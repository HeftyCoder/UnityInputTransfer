using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class AccelerometerInput : Vector3Input
{
    public override void SetUp(InputDevice device, InputEventPtr ptr)
    {
        var acc = (Accelerometer)device;
        value = acc.acceleration.ReadValue();
    }
    public override void QueueInput(InputDevice device)
    {
        var acc = (Accelerometer)device;
        using (StateEvent.From(acc, out InputEventPtr ptr))
        {
            acc.acceleration.WriteValueIntoEvent(value, ptr);
            InputSystem.QueueEvent(ptr);
        }
    }
}

