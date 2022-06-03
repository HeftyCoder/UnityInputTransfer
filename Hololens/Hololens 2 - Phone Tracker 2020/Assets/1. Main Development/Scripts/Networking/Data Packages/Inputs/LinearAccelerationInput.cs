using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class LinearAccelerationInput : Vector3Input
{
    public override void SetUp(InputDevice device)
    {
        var la = (LinearAccelerationSensor)device;
        value = la.acceleration.ReadValue();
    }
    public override void QueueInput(InputDevice device)
    {
        var la = (LinearAccelerationSensor)device;
        using (StateEvent.From(la, out InputEventPtr ptr))
        {
            la.acceleration.WriteValueIntoEvent(value, ptr);
            InputSystem.QueueEvent(ptr);
        }
    }
}
