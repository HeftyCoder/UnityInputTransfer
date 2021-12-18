using System;
using System.Collections.Generic;
using Barebones.Networking;
public class SubscriptionData : SerializablePacket
{
    public List<DeviceIdentificationData> devices = new List<DeviceIdentificationData>();
    
    public SubscriptionData() { }
    public SubscriptionData(List<DeviceIdentificationData> devices) => this.devices = devices;
    public SubscriptionData(IEnumerable<DeviceIdentificationData> devices)
    {
        this.devices.Clear();
        foreach (var data in devices)
            this.devices.Add(data);
    }
    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        writer.Write(devices.Count);
        foreach (var device in devices)
            writer.Write(device);
    }
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        var count = reader.ReadInt32();
        devices.Clear();
        for (int i = 0; i < count; i++)
        {
            var data = new DeviceIdentificationData();
            var device = reader.ReadPacket(data);
            devices.Add(device);
        }
    }

    
}
