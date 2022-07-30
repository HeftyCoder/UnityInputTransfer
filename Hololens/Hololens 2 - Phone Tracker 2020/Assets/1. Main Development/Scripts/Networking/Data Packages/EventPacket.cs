using System;
using System.Collections.Generic;
using Barebones.Networking;

public class EventPacket : SerializablePacket
{
    public byte[] bytes = new byte[0];

    private IMessage message;

    public EventPacket() { }
    public EventPacket(short opcode, ISerializablePacket packet)
    {
        message = MessageHelper.Create(opcode, packet);
    }
    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        var bytes = message.ToBytes();
        writer.Write(bytes.Length);
        writer.Write(bytes);
    }
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        var count = reader.ReadInt32();
        bytes = reader.ReadBytes(count);
    }

    
}
