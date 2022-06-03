using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using System.Linq;
using Barebones.Networking;

public static class InputSystemExtensions
{
    private static int keyLength = 112; // android breaks this? ... Method Access
    private static List<Key> keyHelperList = new List<Key>();
    private static Dictionary<string, Type> availableConnectionTypes;
    static InputSystemExtensions()
    {
        availableConnectionTypes = new Dictionary<string, Type>()
        {
            {nameof(Gamepad), typeof(Gamepad)},
            {nameof(Accelerometer), typeof(Accelerometer)},
            {nameof(AmbientTemperatureSensor), typeof(AmbientTemperatureSensor)},
            {nameof(AttitudeSensor), typeof(AttitudeSensor)},
            {nameof(GravitySensor), typeof(GravitySensor)},
            {nameof(UnityEngine.InputSystem.Gyroscope), typeof(UnityEngine.InputSystem.Gyroscope)},
            {nameof(HumiditySensor), typeof(HumiditySensor)},
            {nameof(Keyboard), typeof(Keyboard)},
            {nameof(LightSensor), typeof(LightSensor)},
            {nameof(LinearAccelerationSensor), typeof(LinearAccelerationSensor)},
            {nameof(MagneticFieldSensor), typeof(MagneticFieldSensor)},
            {nameof(Mouse), typeof(Mouse)},
            {nameof(PressureSensor), typeof(PressureSensor)},
            {nameof(ProximitySensor), typeof(ProximitySensor)},
            {nameof(StepCounter), typeof(StepCounter)},
            {nameof(Touchscreen), typeof(Touchscreen)},
        };
    }
    // android breaks this? ... Method Access Exception
    private static int GetKeyEnumDistinctCount() => ((int[])Enum.GetValues(typeof(Key))).Distinct().Count();
    /// <summary>
    /// Gets Pressed Keys.This isn't thread safe.
    /// </summary>
    public unsafe static Key[] GetPressedKeys(this KeyboardState state) => GetyPressedKeys(state, keyHelperList);
    /// <summary>
    /// Gets Pressed Keys. This can be used in multiple threads with a different list
    /// </summary>
    public unsafe static Key[] GetyPressedKeys(this KeyboardState state, List<Key> keyHelperList)
    {
        var byteBuffer = state.keys;

        int count = 0;
        keyHelperList.Clear();
        while (count < keyLength)
        {
            var value = *byteBuffer;

            for (int i = 0; i < 8; i++)
            {
                //if ((value & (1 << i)) == 1) // For some reason, this doesn't work.. -_-
                var check = (value & (0x1 << i));
                if (((value >> i) & 1) == 1)
                    keyHelperList.Add((Key)count);
                count++;

                if (count >= keyLength)
                    break;
            }

            byteBuffer++;
        }

        var result = new Key[keyHelperList.Count];
        for (int i = 0; i < result.Length; i++)
            result[i] = keyHelperList[i];
        return result;
    }

    public static InputDevice GetDeviceAssignable(string className)
    {
        if (!availableConnectionTypes.TryGetValue(className, out Type value))
            return null;
        return InputSystem.GetDevice(value);
    }
    public static string GetConnectingDeviceLayout(this InputDevice device)
    {
        if (device is Mouse)
            return nameof(Mouse);
        else if (device is Keyboard)
            return nameof(Keyboard);
        else if (device is Touchscreen)
            return nameof(Touchscreen);
        else if (device is Gamepad)
            return nameof(Gamepad);
        else if (device is Accelerometer)
            return nameof(Accelerometer);
        else if (device is UnityEngine.InputSystem.Gyroscope)
            return nameof(UnityEngine.InputSystem.Gyroscope);
        else if (device is GravitySensor)
            return nameof(GravitySensor);
        else if (device is AttitudeSensor)
            return nameof(AttitudeSensor);
        else if (device is LinearAccelerationSensor)
            return nameof(LinearAccelerationSensor);
        else if (device is MagneticFieldSensor)
            return nameof(MagneticFieldSensor);
        else if (device is LightSensor)
            return nameof(LightSensor);
        else if (device is PressureSensor)
            return nameof(PressureSensor);
        else if (device is ProximitySensor)
            return nameof(ProximitySensor);
        else if (device is HumiditySensor)
            return nameof(HumiditySensor);
        else if (device is AmbientTemperatureSensor)
            return nameof(AmbientTemperatureSensor);
        else if (device is StepCounter)
            return nameof(StepCounter);
        return null;
    }
    //This should generally not be acceptible. I should only send byte buffers / arrays and decode through them.
    //Documentation is kind of hard to find for this, so I opted for the general approach.
    #region Input System State Serialization
    public static void Write(this EndianBinaryWriter writer, MouseState state)
    {
        uint buttons = state.buttons;
        writer.Write(buttons);
        uint clicks = state.clickCount;
        writer.Write(clicks);
        writer.Write(state.delta);
        writer.Write(state.position);
        writer.Write(state.scroll);
    }
    public static MouseState ReadMouseState(this EndianBinaryReader reader)
    {
        var state = new MouseState();
        state.buttons = (ushort)reader.ReadUInt32();
        state.clickCount = (ushort)reader.ReadInt32();
        state.delta = reader.ReadVector2();
        state.position = reader.ReadVector2();
        state.scroll = reader.ReadVector2();
        return state;
    }
    public static void Write(this EndianBinaryWriter writer, TouchState state)
    {
        writer.Write(state.delta);
        writer.Write(state.flags);
        writer.Write(state.phaseId);
        writer.Write(state.position);
        writer.Write(state.pressure);
        writer.Write(state.radius);
        writer.Write(state.startPosition);
        writer.Write(state.startTime);
        writer.Write(state.tapCount);
        writer.Write(state.touchId);
    }

    public static void Write(this EndianBinaryWriter writer, KeyboardState state)
    {
        var pressed = state.GetPressedKeys();
        writer.Write(pressed.Length);
        foreach (var key in pressed)
            writer.Write((int)key);
    }
    public static KeyboardState ReadKeyboardState(this EndianBinaryReader reader)
    {
        var count = reader.ReadInt32();
        var pressed = new Key[count];
        for (int i = 0; i < count; i++)
        {
            var key = (Key)reader.ReadInt32();
            pressed[i] = key;
        }
        return new KeyboardState(pressed);
    }
    public static TouchState ReadTouchState(this EndianBinaryReader reader)
    {
        var state = new TouchState()
        {
            delta = reader.ReadVector2(),
            flags = reader.ReadByte(),
            phaseId = reader.ReadByte(),
            position = reader.ReadVector2(),
            pressure = reader.ReadSingle(),
            radius = reader.ReadVector2(),
            startPosition = reader.ReadVector2(),
            startTime = reader.ReadDouble(),
            tapCount = reader.ReadByte(),
            touchId = reader.ReadInt32()
        };
        return state;
    }
    public static void Write(this EndianBinaryWriter writer, GamepadState state)
    {
        writer.Write(state.buttons);
        writer.Write(state.leftStick);
        writer.Write(state.leftTrigger);
        writer.Write(state.rightStick);
        writer.Write(state.rightTrigger);
    }
    public static GamepadState ReadGamepad(this EndianBinaryReader reader)
    {
        var state = new GamepadState();
        state.buttons = reader.ReadUInt32();
        state.leftStick = reader.ReadVector2();
        state.leftTrigger = reader.ReadSingle();
        state.rightStick = reader.ReadVector2();
        state.rightTrigger = reader.ReadSingle();
        return state;
    }

    public static void Write(this EndianBinaryWriter writer, Vector2 vec2)
    {
        writer.Write(vec2.x);
        writer.Write(vec2.y);
    }
    public static Vector2 ReadVector2(this EndianBinaryReader reader)
    {
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        return new Vector2(x, y);
    }
    public static void Write(this EndianBinaryWriter writer, Vector3 vec3)
    {
        writer.Write(vec3.x);
        writer.Write(vec3.y);
        writer.Write(vec3.z);
    }
    public static Vector3 ReadVector3(this EndianBinaryReader reader)
    {
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        var z = reader.ReadSingle();
        return new Vector3(x, y, z);
    }
    public static void Write(this EndianBinaryWriter writer, Quaternion quat)
    {
        writer.Write(quat.x);
        writer.Write(quat.y);
        writer.Write(quat.z);
        writer.Write(quat.w);
    }
    public static Quaternion ReadQuaternion(this EndianBinaryReader reader)
    {
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        var z = reader.ReadSingle();
        var w = reader.ReadSingle();
        return new Quaternion(x, y, z, w);
    }
    #endregion
}
