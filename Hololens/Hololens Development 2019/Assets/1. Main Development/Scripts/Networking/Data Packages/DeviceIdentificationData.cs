using System;
using Barebones.Networking;
using UnityEngine.InputSystem;

public class DeviceIdentificationData : SerializablePacket
{
    public int id;
    public string layout;
    public string name;
    
    public DeviceIdentificationData() { }

    public DeviceIdentificationData(InputDevice device)
    {
        var id = device.deviceId;
        var name = device.name;
        var layout = device.layout;
        Reset(id, name, layout);
    }
    public DeviceIdentificationData(int id, string layout, string name)
    {
        Reset(id, layout, name);
    }
    public void Reset(int id, string layout, string name)
    {
        this.id = id;
        this.layout = layout;
        this.name = name;
    }

    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        writer.Write(id);
        writer.Write(layout);
        writer.Write(name);
    }
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        id = reader.ReadInt32();
        layout = reader.ReadString();
        name = reader.ReadString();
    }
}
