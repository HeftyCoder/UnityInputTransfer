using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PressureInput : SingleInput
{
    public override void SetUp(InputDevice device, InputEventPtr ptr)
    {
        var pr = (PressureSensor)device;
        value = pr.atmosphericPressure.ReadValue();
    }
    public override void QueueInput(InputDevice device)
    {
        var p = (PressureSensor)device;
        using (StateEvent.From(p, out InputEventPtr ptr))
        {
            p.atmosphericPressure.WriteValueIntoEvent(value, ptr);
            InputSystem.QueueEvent(ptr);
        }
    }
}