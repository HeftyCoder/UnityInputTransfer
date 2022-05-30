using System;
using System.Collections.Generic;
using Barebones.Networking;
using UnityEngine.InputSystem;

public abstract class BaseInput : SerializablePacket 
{
    public abstract void SetUp(InputDevice device); 
    public abstract void QueueInput(InputDevice device);
}

