using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class HumidityInput : SingleInput
{
    public override void SetUp(InputDevice device, InputEventPtr ptr)
    {
        var h = (HumiditySensor)device;
        value = h.relativeHumidity.ReadValue();
    }
    public override void QueueInput(InputDevice device)
    {
        var h = (HumiditySensor)device;
        using (StateEvent.From(h, out InputEventPtr ptr))
        {
            h.relativeHumidity.WriteValueIntoEvent(value,ptr);
            InputSystem.QueueEvent(ptr);
        }
    }
}
