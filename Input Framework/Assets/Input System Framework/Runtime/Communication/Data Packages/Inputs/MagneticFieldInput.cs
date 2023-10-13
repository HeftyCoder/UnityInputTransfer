using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class MagneticFieldInput : Vector3Input
{
    public override void SetUp(InputDevice device, InputEventPtr ptr)
    {
        var m = (MagneticFieldSensor)device;
        value = m.magneticField.ReadValue();
    }
    public override void QueueInput(InputDevice device)
    {
        var mf = (MagneticFieldSensor)device;
        using (StateEvent.From(mf, out InputEventPtr ptr))
        {
            mf.magneticField.WriteValueIntoEvent(value, ptr);
            InputSystem.QueueEvent(ptr);
        }
    }
}
