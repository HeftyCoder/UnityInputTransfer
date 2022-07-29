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

    public void SendEvent(EventIdentifier e, ISerializablePacket packet)
    {
        client.ClientSocket.SendMessage(e.Id, packet);
    }
}
