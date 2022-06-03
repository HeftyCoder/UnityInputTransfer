using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class StepCounterInput : IntegerInput
{
    public override void SetUp(InputDevice device)
    {
        var sc = (StepCounter)device;
        value = sc.stepCounter.ReadValue();
    }
    public override void QueueInput(InputDevice device)
    {
        var s = (StepCounter)device;
        using (StateEvent.From(s, out InputEventPtr ptr))
        {
            s.stepCounter.WriteValueIntoEvent(value, ptr);
            InputSystem.QueueEvent(ptr);
        }
    }
}
