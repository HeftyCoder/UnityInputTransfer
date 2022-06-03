using Barebones.Networking;
using UnityEngine.InputSystem;

public class NoInput : BaseInput
{
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
    }

    public override void QueueInput(InputDevice device)
    {
    }

    public override void SetUp(InputDevice device)
    {
    }

    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
    }
}
