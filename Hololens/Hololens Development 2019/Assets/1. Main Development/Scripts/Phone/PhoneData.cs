using System;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
using UnityEngine.InputSystem;

public class PhoneData : SerializablePacket
{
    public IReadOnlyCollection<DeviceData> DeviceChanges { get; private set; }
    public byte[] InputEvents { get; private set; }

    public PhoneData() { }
    public PhoneData(IReadOnlyCollection<DeviceData> deviceChanges, byte[] inputEvents) => Reset(deviceChanges, inputEvents);
    public void Reset(IReadOnlyCollection<DeviceData> deviceChanges, byte[] inputEvents)
    {
        DeviceChanges = deviceChanges;
        InputEvents = inputEvents;
    }
    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        writer.Write(DeviceChanges.Count);
        foreach (var change in DeviceChanges)
        {
            writer.Write(change);
        }

        writer.Write(InputEvents.Length);
        writer.Write(InputEvents);
    }
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        var collection = new List<DeviceData>();

        var count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            var data = new DeviceData();
            reader.ReadPacket(data);
            collection.Add(data);
        }

        DeviceChanges = collection;

        count = reader.ReadInt32();
        InputEvents = reader.ReadBytes(count);
    }

}
