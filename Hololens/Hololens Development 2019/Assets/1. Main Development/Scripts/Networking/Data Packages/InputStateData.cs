using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Barebones.Networking;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

//We're basically creating a C Union to store all the necessary input data per given type
//I hope this is not unecessary, because I couldn't find how to convert bytes to state from the documentation.
//For now, I'm leaving it as is
public enum InputDataType { Integer, Float, Double, Vector2, Vector3, GamepadState, MouseState, KeyboardState, TouchState }

[StructLayout(LayoutKind.Explicit)]
public struct InputStateData : ISerializablePacket
{
    [FieldOffset(0)] public InputDataType dataType;
    [FieldOffset(4)] public int integer;
    [FieldOffset(4)] public float single;
    [FieldOffset(4)] public double @double;
    [FieldOffset(4)] public Vector2 vec2;
    [FieldOffset(4)] public Vector3 vec3;
    [FieldOffset(4)] public GamepadState gamepadState;
    [FieldOffset(4)] public MouseState mouseState;
    [FieldOffset(4)] public KeyboardState keyboardState;
    [FieldOffset(4)] public TouchState touchState;

    public InputStateData(InputDataType dataType)
    {
        this.dataType = dataType;
        single = 0;
        @double = 0;
        vec2 = Vector2.zero;
        vec3 = Vector3.zero;
        gamepadState = new GamepadState();
        mouseState = new MouseState();
        keyboardState = new KeyboardState();
        touchState = new TouchState();
        integer = 0;
    }
    public void ToBinaryWriter(EndianBinaryWriter writer)
    {
        writer.Write((int)dataType);
        switch (dataType)
        {
            case InputDataType.Integer:
                writer.Write(integer);
                break;
            case InputDataType.Float:
                writer.Write(single);
                break;
            case InputDataType.Double:
                writer.Write(@double);
                break;
            case InputDataType.Vector2:
                writer.Write(vec2);
                break;
            case InputDataType.Vector3:
                writer.Write(vec3);
                break;
            case InputDataType.GamepadState:
                writer.Write(gamepadState);
                break;
            case InputDataType.MouseState:
                writer.Write(mouseState);
                break;
            case InputDataType.KeyboardState:
                writer.Write(keyboardState);
                break;
            case InputDataType.TouchState:
                writer.Write(touchState);
                break;
        }
    }
    public void FromBinaryReader(EndianBinaryReader reader)
    {
        dataType = (InputDataType)reader.ReadInt32();
        switch (dataType)
        {
            case InputDataType.Integer:
                integer = reader.ReadInt32();
                break;
            case InputDataType.Float:
                single = reader.ReadSingle();
                break;
            case InputDataType.Double:
                @double = reader.ReadDouble();
                break;
            case InputDataType.Vector2:
                vec2 = reader.ReadVector2();
                break;
            case InputDataType.Vector3:
                vec3 = reader.ReadVector3();
                break;
            case InputDataType.GamepadState:
                gamepadState = reader.ReadGamepad();
                break;
            case InputDataType.MouseState:
                mouseState = reader.ReadMouseState();
                break;
            case InputDataType.KeyboardState:
                keyboardState = reader.ReadKeyboardState();
                break;
            case InputDataType.TouchState:
                touchState = reader.ReadTouchState();
                break;
        }
    }

    public byte[] ToBytes() => this.ToBytesExtension();
}
