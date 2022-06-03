using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class TouchscreenInput : BaseInput
{
    public TouchState touchState;

    public override void SetUp(InputDevice device) => device.CopyState(out touchState);
    public override void QueueInput(InputDevice device) => InputSystem.QueueStateEvent(device, touchState);

    public override void ToBinaryWriter(EndianBinaryWriter writer) => writer.Write(touchState);
    public override void FromBinaryReader(EndianBinaryReader reader) => touchState = reader.ReadTouchState();
}
