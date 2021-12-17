using System;
using System.Collections.Generic;
using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
public class DeviceData : SerializablePacket
{
    public int id;
    public string layout;
    public string name;
    public InputDeviceChange deviceChange;
    public InputStateData stateData;

    public DeviceData() { }
    public DeviceData(InputDevice device, InputDeviceChange change, InputStateData data) => Reset(device, change, data);
    public DeviceData(int id, string layout, string name, InputDeviceChange deviceChange, InputStateData data) 
        => Reset(id, layout, name, deviceChange, data);
    public void Reset(InputDevice device, InputDeviceChange change, InputStateData data)
    {
        var id = device.deviceId;
        var layout = device.layout;
        var name = device.name;
        Reset(id, layout, name, change, data);
    }
    public void Reset(int id, string layout, string name, InputDeviceChange deviceChange, InputStateData data)
    {
        this.id = id;
        this.layout = layout;
        this.name = name;
        this.deviceChange = deviceChange;
        this.stateData = data;
    }

    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        writer.Write(id);
        writer.Write(layout);
        writer.Write(name);
        writer.Write((int)deviceChange);
        writer.Write(stateData);
    }
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        id = reader.ReadInt32();
        layout = reader.ReadString();
        name = reader.ReadString();
        deviceChange = (InputDeviceChange)reader.ReadInt32();
        stateData = new InputStateData();
        reader.ReadPacket(stateData);
    }

}
