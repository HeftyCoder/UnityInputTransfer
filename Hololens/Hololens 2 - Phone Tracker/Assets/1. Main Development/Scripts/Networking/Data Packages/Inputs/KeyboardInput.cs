using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class KeyboardInput : BaseInput
{
    public KeyboardState keyboardState;
    public override void SetUp(InputDevice device)
    {
        device.CopyState(out keyboardState);
    }
    public override void QueueInput(InputDevice device)
    {
        var keyboard = (Keyboard)device;
        InputSystem.QueueStateEvent(keyboard, keyboardState);
    }

    public override void ToBinaryWriter(EndianBinaryWriter writer) => writer.Write(keyboardState);
    public override void FromBinaryReader(EndianBinaryReader reader) => keyboardState = reader.ReadKeyboardState();
}