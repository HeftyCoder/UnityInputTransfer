using System;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;

public abstract class EventManager : MonoBehaviour
{
    public abstract void SetEventHandler(EventIdentifier e, Action<IIncommingMessage> onEventRaised);
    public abstract void RemoveEventHandler(EventIdentifier e);
}
