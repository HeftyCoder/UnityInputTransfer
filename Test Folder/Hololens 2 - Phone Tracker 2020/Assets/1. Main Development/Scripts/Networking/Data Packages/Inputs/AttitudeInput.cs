using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine;
using Barebones.Networking;

public class AttitudeInput : BaseInput
{
    public Quaternion value;

    public override void SetUp(InputDevice device, InputEventPtr ptr)
    {
        var att = (AttitudeSensor)device;
        value = att.attitude.ReadValue();
    }
    public override void QueueInput(InputDevice device)
    {
        var att = (AttitudeSensor)device;
        using (StateEvent.From(att, out InputEventPtr ptr))
        {
            att.attitude.WriteValueIntoEvent(value, ptr);
            InputSystem.QueueEvent(ptr);
        }
    }

    public override void ToBinaryWriter(EndianBinaryWriter writer) => writer.Write(value);
    public override void FromBinaryReader(EndianBinaryReader reader) => value = reader.ReadQuaternion();
}
