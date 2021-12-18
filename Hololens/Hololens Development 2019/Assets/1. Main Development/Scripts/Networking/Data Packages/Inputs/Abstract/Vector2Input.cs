using Barebones.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Vector2Input : BaseInput 
{
    public Vector2 value;

    public override void ToBinaryWriter(EndianBinaryWriter writer) => writer.Write(value);
    public override void FromBinaryReader(EndianBinaryReader reader) => value = reader.ReadVector2();
}
