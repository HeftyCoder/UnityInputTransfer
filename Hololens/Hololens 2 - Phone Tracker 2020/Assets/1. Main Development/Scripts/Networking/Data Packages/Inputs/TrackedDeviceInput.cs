using Barebones.Networking;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine;

public class TrackedDeviceInput : BaseInput
{
    public Vector3 position;
    public Quaternion rotation;

    public TrackedDeviceInput() { }
    public TrackedDeviceInput(Vector3 position, Quaternion rotation) => SetValues(position, rotation);
    public void SetValues(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
    public override void SetUp(InputDevice device)
    {
        var tr = (TrackedDevice)device;
        position = tr.devicePosition.ReadValue();
        rotation = tr.deviceRotation.ReadValue();
    }
    public override void QueueInput(InputDevice device)
    {
        var tr = (TrackedDevice)device;
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