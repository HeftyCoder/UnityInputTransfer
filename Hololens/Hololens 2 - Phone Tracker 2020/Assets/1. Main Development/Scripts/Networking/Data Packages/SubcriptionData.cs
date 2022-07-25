using System;
using System.Collections.Generic;
using Barebones.Networking;
public class SubscriptionData : SerializablePacket
{
    public IList<DeviceDescription> devices = new List<DeviceDescription>();
    public ArucoBoardLayout arucoLayout = new ArucoBoardLayout();

    public SubscriptionData() { }
    public SubscriptionData(IList<DeviceDescription> devices) => this.devices = devices;
    public SubscriptionData(IEnumerable<DeviceDescription> devices)
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

        writer.Write(arucoLayout);
    }
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        var count = reader.ReadInt32();
        devices.Clear();
        for (int i = 0; i < count; i++)
        {
            var data = new DeviceDescription();
            var device = reader.ReadPacket(data);
            devices.Add(device);
        }

        arucoLayout = new ArucoBoardLayout();
        reader.ReadPacket(arucoLayout);
    }

    
}
