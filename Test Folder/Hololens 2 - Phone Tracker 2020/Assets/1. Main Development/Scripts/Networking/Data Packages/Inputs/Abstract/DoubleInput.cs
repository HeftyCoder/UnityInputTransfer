using System;
using System.Collections.Generic;
using Barebones.Networking;

public abstract class DoubleInput : BaseInput
{
    public double value;

    public override void ToBinaryWriter(EndianBinaryWriter writer) => writer.Write(value);
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        value = reader.ReadDouble();
    }
}
