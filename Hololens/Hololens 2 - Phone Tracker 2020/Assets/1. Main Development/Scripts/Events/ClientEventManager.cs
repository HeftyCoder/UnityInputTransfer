using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Barebones.Networking;
public class ClientEventManager : EventManager
{
    [SerializeField] PhoneClient client;
    public override void SetEventHandler(EventIdentifier e, Action<IIncommingMessage> onEventRaised) =>
        client.ClientSocket.SetHandler(e.Id, CreateHandler(onEventRaised));
    public override void RemoveEventHandler(EventIdentifier e)
    {
        //does nothing for now
    }
    private IncommingMessageHandler CreateHandler(Action<IIncommingMessage> onEventRaised)
    {
        IncommingMessageHandler handler = (message) => onEventRaised?.Invoke(message);
        return handler;
    }

    public void SendImmediate(EventIdentifier e, ISerializablePacket packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableSequenced)
    {
        client.ClientSocket.SendMessage(e.Id, packet, deliveryMethod);
    }
    public void Send(EventIdentifier e, ISerializablePacket packet)
    {
        var ev = new EventPacket(e.Id, packet);
        client.Events.Add(ev);
    }
}
