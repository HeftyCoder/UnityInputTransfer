﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
using UnityEngine.InputSystem;

public class DeviceData : SerializablePacket
{
    public IList<InputData> inputDatas = new List<InputData>();
    public IList<EventPacket> events = new List<EventPacket>();
    public PingMessage ping;
    public double latency;
    public double networkTimestamp;
    public DeviceData() { }
    public DeviceData(IEnumerable<InputData> deviceChanges, IEnumerable<EventPacket> eventPackets) => Reset(deviceChanges, eventPackets);
    public void Reset(IEnumerable<InputData> deviceChanges, IEnumerable<EventPacket> eventPackets)
    {
        inputDatas.Clear();
        foreach (var data in deviceChanges)
            inputDatas.Add(data);

        events.Clear();
        foreach (var ev in eventPackets)
            events.Add(ev);

    }
    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        writer.Write(inputDatas.Count);
        foreach (var change in inputDatas)
        {
            writer.Write(change);
        }

        writer.Write(events.Count);
        foreach (var ev in events)
            writer.Write(ev);

        writer.Write(ping);
        writer.Write(latency);
        writer.Write(networkTimestamp);
    }
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        inputDatas.Clear();
        var count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            var data = new InputData();
            reader.ReadPacket(data);
            inputDatas.Add(data);
        }

        events.Clear();
        count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            var ev = new EventPacket();
            reader.ReadPacket(ev);
            events.Add(ev);
        }

        ping = new PingMessage();
        reader.ReadPacket(ping);
        latency = reader.ReadDouble();
        networkTimestamp = reader.ReadDouble();
    }

}
