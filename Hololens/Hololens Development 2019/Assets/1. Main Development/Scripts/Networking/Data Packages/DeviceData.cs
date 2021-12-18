using System;
using System.Collections.Generic;
using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
public class DeviceData : SerializablePacket
{
    public DeviceIdentificationData identificationData;
    public InputDeviceChange deviceChange;
    public InputStateData stateData;

    public DeviceData() { }
    public DeviceData(InputDevice device, InputDeviceChange change, InputStateData data) => Reset(device, change, data);
    public DeviceData(DeviceIdentificationData idData, InputDeviceChange deviceChange, InputStateData data) 
        => Reset(idData, deviceChange, data);
    public void Reset(InputDevice device, InputDeviceChange change, InputStateData data)
    {
        var idData = new DeviceIdentificationData(device);
        Reset(idData, change, data);
    }
    public void Reset(DeviceIdentificationData identificationData, InputDeviceChange deviceChange, InputStateData data)
    {
        this.identificationData = identificationData;
        this.deviceChange = deviceChange;
        this.stateData = data;
    }

    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        writer.Write(identificationData);
        writer.Write((int)deviceChange);
        writer.Write(stateData);
    }
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        identificationData = new DeviceIdentificationData();
        reader.ReadPacket(identificationData);
        deviceChange = (InputDeviceChange)reader.ReadInt32();
        stateData = new InputStateData();
        reader.ReadPacket(stateData);
    }

}
