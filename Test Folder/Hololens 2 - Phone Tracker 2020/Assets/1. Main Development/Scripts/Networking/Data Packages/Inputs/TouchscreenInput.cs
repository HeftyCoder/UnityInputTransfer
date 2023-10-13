using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using System.Collections.Generic;
public class TouchscreenInput : BaseInput
{
    public List<TouchState> touchStates = new List<TouchState>();

    public override void SetUp(InputDevice device, InputEventPtr ptr)
    {
        touchStates.Clear();
        var screen = (Touchscreen)device;
        foreach (var touch in screen.touches)
        {
            touchStates.Add(touch.ReadValue());
        }
    }

    public override void QueueInput(InputDevice device)
    {
        foreach (var touchState in touchStates)
            InputSystem.QueueStateEvent(device, touchState);
    }

    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        writer.Write(touchStates.Count);
        foreach (var state in touchStates)
            writer.Write(state);
    }
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        touchStates.Clear();
        var count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            var state = reader.ReadTouchState();
            touchStates.Add(state);
        }
    }
}
