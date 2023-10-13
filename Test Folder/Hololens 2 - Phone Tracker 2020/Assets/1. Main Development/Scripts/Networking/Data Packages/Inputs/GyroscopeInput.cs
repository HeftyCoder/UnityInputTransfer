using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class GyroscopeInput : Vector3Input
{
    public override void SetUp(InputDevice device, InputEventPtr ptr)
    {
        var gr = (Gyroscope)device;
        value = gr.angularVelocity.ReadValue();
    }
    public override void QueueInput(InputDevice device)
    {
        var gyro = (Gyroscope)device;
        using (StateEvent.From(gyro, out InputEventPtr ptr))
        {
            gyro.angularVelocity.WriteValueIntoEvent(value, ptr);
            InputSystem.QueueEvent(ptr);
        }
    }
}
