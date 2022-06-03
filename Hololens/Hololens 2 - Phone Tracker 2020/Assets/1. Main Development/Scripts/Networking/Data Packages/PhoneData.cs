using System;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
using UnityEngine.InputSystem;

public class PhoneData : SerializablePacket
{
    public IList<InputData> inputDatas = new List<InputData>();
    
    public PhoneData() { }
    public PhoneData(IEnumerable<InputData> deviceChanges) => Reset(deviceChanges);
    public void Reset(IEnumerable<InputData> deviceChanges)
    {
        inputDatas.Clear();
        foreach (var data in deviceChanges)
            inputDatas.Add(data);
    }
    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        writer.Write(inputDatas.Count);
        foreach (var change in inputDatas)
        {
            writer.Write(change);
        }
    }
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        inputDatas.Clear();
        var count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            var data = new InputData();
            reader.ReadPacket(data);
            inputDatas.Add(data);
        }
    }

}
