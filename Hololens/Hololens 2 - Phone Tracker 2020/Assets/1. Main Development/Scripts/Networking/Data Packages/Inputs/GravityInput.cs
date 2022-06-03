using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class GravityInput : Vector3Input
{
    public override void SetUp(InputDevice device)
    {
        var gr = (GravitySensor)device;
        value = gr.gravity.ReadValue();
    }
    public override void QueueInput(InputDevice device)
    {
        var gr = (GravitySensor)device;
        using (StateEvent.From(gr, out InputEventPtr ptr))
        {
            gr.gravity.WriteValueIntoEvent(value, ptr);
            InputSystem.QueueEvent(ptr);
        }
    }
}
