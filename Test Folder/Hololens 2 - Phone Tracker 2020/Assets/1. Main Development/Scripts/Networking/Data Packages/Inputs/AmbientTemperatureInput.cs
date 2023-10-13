using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class AmbientTemperatureInput : SingleInput
{
    public override void SetUp(InputDevice device, InputEventPtr ptr)
    {
        var at = (AmbientTemperatureSensor)device;
        value = at.ambientTemperature.ReadValue();
    }
    public override void QueueInput(InputDevice device)
    {
        var at = (AmbientTemperatureSensor)device;
        using (StateEvent.From(at, out InputEventPtr ptr))
        {
            at.ambientTemperature.WriteValueIntoEvent(value, ptr);
            InputSystem.QueueEvent(ptr);
        }
    }
}