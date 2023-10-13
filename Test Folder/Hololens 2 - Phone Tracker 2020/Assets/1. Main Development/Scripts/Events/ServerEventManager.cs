using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
using System;
public class ServerEventManager : EventManager
{
    [SerializeField] DeviceServer server;

    public override void SetEventHandler(EventIdentifier e, Action<IIncommingMessage> onEventRaised) =>
        server.Operations.Add(e.Id, onEventRaised);

    public override void RemoveEventHandler(EventIdentifier e) => server.Operations.Remove(e.Id);

}
