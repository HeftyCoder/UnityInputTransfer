using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Barebones.Networking;

[DisallowMultipleComponent, RequireComponent(typeof(Transport))]
public class BaseTransportSocket : MonoBehaviour
{
    public static int GetChannelId(DeliveryMethod method)
    {
        int channelId = -1;
        switch (method)
        {
            case DeliveryMethod.Reliable:
            case DeliveryMethod.ReliableSequenced:
                channelId = Channels.Reliable;
                break;
            case DeliveryMethod.Unreliable:
            default:
                channelId = Channels.Unreliable;
                break;
        }
        return channelId;
    }
    [SerializeField] protected Transport transport;

    public Transport Transport => transport;

}
