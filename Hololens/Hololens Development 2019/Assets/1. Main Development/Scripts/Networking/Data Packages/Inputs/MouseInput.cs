using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class MouseInput : BaseInput
{
    public MouseState mouseState;
    public override void SetUp(InputDevice device) => device.CopyState(out mouseState);
    public override void QueueInput(InputDevice device) => InputSystem.QueueStateEvent(device, mouseState);
    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        throw new System.NotImplementedException();
    }
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        throw new System.NotImplementedException();
    }
}
