using System;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;

[Serializable]
public class ArucoBoardLayout : SerializablePacket
{
    public List<ArucoBoardLayoutItem> items = new List<ArucoBoardLayoutItem>();

    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        items = new List<ArucoBoardLayoutItem>();
        var count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            var item = new ArucoBoardLayoutItem();
            reader.ReadPacket(item);
            items.Add(item);
        }
    }

    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        writer.Write(items.Count);
        foreach (var item in items)
            writer.Write(item);
    }
} 
