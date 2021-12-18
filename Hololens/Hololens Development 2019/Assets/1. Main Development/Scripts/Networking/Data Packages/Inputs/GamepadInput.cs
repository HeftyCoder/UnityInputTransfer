using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class GamepadInput : BaseInput
{
    public GamepadState state;

    public override void SetUp(InputDevice device)
    {
        var gp = (Gamepad)device;
        gp.CopyState(out state);
    }
    public override void QueueInput(InputDevice device)
    {
        InputSystem.QueueStateEvent(device, state);
    }

    public override void ToBinaryWriter(EndianBinaryWriter writer) => writer.Write(state);
    public override void FromBinaryReader(EndianBinaryReader reader) => reader.ReadGamepad();
}
