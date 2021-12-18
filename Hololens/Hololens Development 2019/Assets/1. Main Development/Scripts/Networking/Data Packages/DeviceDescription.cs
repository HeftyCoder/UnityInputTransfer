using System;
using Barebones.Networking;
using UnityEngine.InputSystem.Layouts;

[Serializable]
public class DeviceDescription : SerializablePacket
{
    [InputControl(layout = "Sensor")]
    public string device;
    public string customName;
    [NonSerialized]
    public int deviceId;

    private string layout;
    private string customDefaultName;
    public string Layout
    {
        get
        {
            if (!string.IsNullOrEmpty(layout))
                return layout;
            layout = device.Substring(1, device.Length - 1);
            return layout;
        }
    }

    public string CustomName
    {
        get
        {
            if (!string.IsNullOrEmpty(customName))
                return customName;
            if (!string.IsNullOrEmpty(customDefaultName))
                return customDefaultName;
            customDefaultName = $"Connection{Layout}";
            return customDefaultName;
        }
    }

    public DeviceDescription() { }
    
    public DeviceDescription(DeviceDescription copyFrom)
    {
        device = copyFrom.device;
        customName = copyFrom.customName;
        deviceId = copyFrom.deviceId;
        layout = copyFrom.layout;
        customDefaultName = copyFrom.customDefaultName;
    }

    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        writer.Write(device);
        writer.Write(customName);
        writer.Write(deviceId);
    }

    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        device = reader.ReadString();
        customName = reader.ReadString();
        deviceId = reader.ReadInt32();
    }
}