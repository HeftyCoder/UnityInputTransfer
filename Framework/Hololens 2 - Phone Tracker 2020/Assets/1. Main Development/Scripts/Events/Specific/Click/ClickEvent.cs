using System;
using System.Collections.Generic;
using Barebones.Networking;
using UnityEngine.EventSystems;
public class ClickEvent : SerializablePacket
{
    public int clickCount;
    public PointerEventData.InputButton button;
    public float clickTime;
    public ClickEvent() { }
    public ClickEvent(PointerEventData data)
    {
        clickCount = data.clickCount;
        button = data.button;
        clickTime = data.clickTime;
    }

    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        writer.Write(clickCount);
        writer.Write((int)button);
        writer.Write(clickTime);
    }
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        clickCount = reader.ReadInt32();
        button = (PointerEventData.InputButton)reader.ReadInt32();
        clickTime = reader.ReadSingle();
    }
}
