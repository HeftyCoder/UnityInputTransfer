using System;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;

[Serializable]
public class ArucoBoardLayout : SerializablePacket
{
    public float size = 0;
    public Vector3 topLeftCorner = Vector3.zero;

    public ArucoBoardLayout() { }
    public ArucoBoardLayout(float size, Vector3 topLeftCorner)
    {
        this.size = size;
        this.topLeftCorner = topLeftCorner;
    }
    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        writer.Write(size);
        writer.Write(topLeftCorner);
    }
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        size = reader.ReadInt32();
        topLeftCorner = reader.ReadVector3();
    }
}
