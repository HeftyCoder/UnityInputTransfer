using System;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;

public class PhoneInputSerialization : SerializablePacket
{
    public Vector3 Acceleration { get; private set; }
    public Vector3 AngularVelocity { get; private set; }
    public Vector3 Gravity { get; private set; }
    public Vector3 LinearAcceleration { get; private set; }
    public Vector2 TouchDelta { get; private set; }
    public int TapCount { get; private set; }

    public override string ToString()
    {
        return $"Acceleration: {Acceleration}\n Angular Velocity: {AngularVelocity}\n Gravity: {Gravity}\n Linear Acceleration: {LinearAcceleration}\n" +
            $"Touch Delta: {TouchDelta}\n Tap Count:{TapCount}";
    }
    public PhoneInputSerialization()
    {

    }
    public PhoneInputSerialization(Vector3 accel, Vector3 angVel, Vector3 grav, Vector3 linAccel, Vector2 touchDelta, int tapCount)
    {
        Initialize(accel, angVel, grav, linAccel, touchDelta, tapCount);
    }
    public void Initialize(Vector3 accel, Vector3 angVel, Vector3 grav, Vector3 linAccel, Vector2 touchDelta, int tapCount)
    {
        Acceleration = accel;
        AngularVelocity = angVel;
        Gravity = grav;
        LinearAcceleration = linAccel;
        TouchDelta = touchDelta;
        TapCount = tapCount;
    }

    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        WriteVector3(Acceleration, writer);
        WriteVector3(AngularVelocity, writer);
        WriteVector3(Gravity, writer);
        WriteVector3(LinearAcceleration, writer);
        WriteVector2(TouchDelta, writer);
        writer.Write(TapCount);
    }
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        Acceleration = ReadVector3(reader);
        AngularVelocity = ReadVector3(reader);
        Gravity = ReadVector3(reader);
        LinearAcceleration = ReadVector3(reader);
        TouchDelta = ReadVector2(reader);
        TapCount = reader.ReadInt32();
    }

    private void WriteVector3(Vector3 vector, EndianBinaryWriter writer)
    {
        writer.Write(vector.x);
        writer.Write(vector.y);
        writer.Write(vector.z);
    }
    private Vector3 ReadVector3(EndianBinaryReader reader)
    {
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        var z = reader.ReadSingle();
        return new Vector3(x, y, z);
    }
    private void WriteVector2(Vector2 vector, EndianBinaryWriter writer)
    {
        writer.Write(vector.x);
        writer.Write(vector.y);
    }
    private Vector2 ReadVector2(EndianBinaryReader reader)
    {
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        return new Vector2(x, y);
    }

    
}
