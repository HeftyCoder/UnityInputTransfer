using Barebones.Networking;
using System;
using System.Collections.Generic;

public abstract class IntegerInput : BaseInput
{
    public int value;
    public override void ToBinaryWriter(EndianBinaryWriter writer) => writer.Write(value);
    public override void FromBinaryReader(EndianBinaryReader reader) => value = reader.ReadInt32();
}
