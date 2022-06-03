using Barebones.Networking;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine;

public class TransformFollowerDeviceInput : BaseInput
{
    public Vector3 position;
    public Quaternion rotation;

    public TransformFollowerDeviceInput() { }
    public TransformFollowerDeviceInput(Vector3 position, Quaternion rotation) => SetValues(position, rotation);
    public void SetValues(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
    public override void SetUp(InputDevice device)
    {
        var tr = (TransformFollowerDevice)device;
        position = tr.devicePosition.ReadValue();
        rotation = tr.deviceRotation.ReadValue();
    }
    public override void QueueInput(InputDevice device)
    {
        var tr = (TransformFollowerDevice)device;
        using (StateEvent.From(tr, out InputEventPtr ptr))
        {
            tr.devicePosition.WriteValueIntoEvent(position, ptr);
            tr.deviceRotation.WriteValueIntoEvent(rotation, ptr);
            InputSystem.QueueEvent(ptr);
        }
    }

    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        writer.Write(position);
        writer.Write(rotation);
    }

    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        position = reader.ReadVector3();
        rotation = reader.ReadQuaternion();
    }
}
