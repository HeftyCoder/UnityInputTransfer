using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class LightInput : SingleInput
{
    public override void SetUp(InputDevice device)
    {
        var l = (LightSensor)device;
        value = l.lightLevel.ReadValue();
    }
    public override void QueueInput(InputDevice device)
    {
        var light = (LightSensor)device;
        using (StateEvent.From(light, out InputEventPtr ptr))
        {
            light.lightLevel.WriteValueIntoEvent(value, ptr);
            InputSystem.QueueEvent(ptr);
        }
    }
}
