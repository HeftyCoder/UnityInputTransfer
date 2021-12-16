using System;
using System.Collections.Generic;
using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
public class DeviceData : SerializablePacket
{
    public int Id { get; private set; }
    public string Layout { get; private set; }
    public string Name { get; private set; }
    public InputDeviceChange DeviceChange { get; private set; }

    public DeviceData() { }
    public DeviceData(InputDevice device, InputDeviceChange change) => Reset(device, change);
    public DeviceData(int id, string layout, string name, InputDeviceChange deviceChange) => Reset(id, layout, name, deviceChange);
    public void Reset(InputDevice device, InputDeviceChange change)
    {
        var id = device.deviceId;
        var layout = device.layout;
        var name = device.name;
        Reset(id, layout, name, change);
    }
    public void Reset(int id, string layout, string name, InputDeviceChange deviceChange)
    {
        Id = id;
        Layout = layout;
        Name = name;
        DeviceChange = deviceChange;
    }

    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        writer.Write(Id);
        writer.Write(Layout);
        writer.Write(Name);
        writer.Write((int)DeviceChange);
    }
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        Id = reader.ReadInt32();
        Layout = reader.ReadString();
        Name = reader.ReadString();
        DeviceChange = (InputDeviceChange)reader.ReadInt32();
    }

}
