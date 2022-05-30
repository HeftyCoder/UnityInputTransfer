using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class ProximityInput : SingleInput
{
    public override void SetUp(InputDevice device)
    {
        var pr = (ProximitySensor)device;
        value = pr.distance.ReadValue();
    }
    public override void QueueInput(InputDevice device)
    {
        var pr = (ProximitySensor)device;
        using (StateEvent.From(pr, out InputEventPtr ptr))
        {
            pr.distance.WriteValueIntoEvent(value, ptr);
            InputSystem.QueueEvent(ptr);
        }
    }
}
