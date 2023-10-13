using System;
using System.Collections.Generic;
using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public abstract class BaseInput : SerializablePacket 
{
    public abstract void SetUp(InputDevice device, InputEventPtr inputPtr); 
    public abstract void QueueInput(InputDevice device);
}

