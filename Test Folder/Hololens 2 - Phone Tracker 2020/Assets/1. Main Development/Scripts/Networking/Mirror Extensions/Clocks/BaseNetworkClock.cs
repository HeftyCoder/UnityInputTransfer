using System;
using System.Collections.Generic;
using Barebones.Networking;
using Mirror;
using System.Runtime.CompilerServices;
public abstract class BaseNetworkClock
{
    public double LocalTime
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => UnityEngine.Time.timeAsDouble;
    }
    public abstract double Time { get; }
    public virtual void Reset() { }
}

[Serializable]
public class PingMessage : SerializablePacket
{
    public double clientTime;
    public PingMessage() { }
    public PingMessage(double clientTime) => this.clientTime = clientTime;
    public override void ToBinaryWriter(EndianBinaryWriter writer) => writer.Write(clientTime);
    public override void FromBinaryReader(EndianBinaryReader reader) => clientTime = reader.ReadDouble();
}

[Serializable]
public class PongMessage : SerializablePacket
{
    public double clientTime;
    public double serverTime;
    public PongMessage() { }
    public PongMessage(double clientTime, double serverTime)
    {
        this.clientTime = clientTime;
        this.serverTime = serverTime;
    }
    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        writer.Write(clientTime);
        writer.Write(serverTime);
    }
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        clientTime = reader.ReadDouble();
        serverTime = reader.ReadDouble();
    }
}
