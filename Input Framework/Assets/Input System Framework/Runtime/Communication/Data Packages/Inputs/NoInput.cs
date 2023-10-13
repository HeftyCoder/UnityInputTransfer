using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
public class NoInput : BaseInput
{
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
    }

    public override void QueueInput(InputDevice device)
    {
    }

    public override void SetUp(InputDevice device, InputEventPtr ptr)
    {
    }

    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
    }
}
