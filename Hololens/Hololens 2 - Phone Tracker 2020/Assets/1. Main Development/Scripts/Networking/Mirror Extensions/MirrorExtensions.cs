using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using kcp2k;
public static class MirrorExtensions
{
    private static Dictionary<Type, Func<Transport, int>> getPortByType;
    private static Dictionary<Type, Action<Transport, int>> setPortByType;
    //because a transport can't for the love of god define a port on its base type

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void InitializePortChange()
    {
        getPortByType = new Dictionary<Type, Func<Transport, int>>
        {
            { typeof(KcpTransport), (transport) => ((KcpTransport)transport).Port},
            { typeof(TelepathyTransport), (transport) => ((TelepathyTransport)transport).port},
            { typeof(MiddlewareTransport), (transport) => {
                    var middle = (MiddlewareTransport)transport;
                    return middle.inner.GetPort();
                }
            },
            { typeof(LatencySimulation), (transport) => {
                    var latencySimulation = (LatencySimulation)transport;
                    return latencySimulation.wrap.GetPort();
                }
            }
        };

        setPortByType = new Dictionary<Type, Action<Transport, int>>
        {
            { typeof(KcpTransport), (transport, port) => ((KcpTransport)transport).Port = (ushort)port},
            { typeof(TelepathyTransport), (transport, port) => ((TelepathyTransport)transport).port = (ushort)port},
            { typeof(MiddlewareTransport), (transport, port) =>
                {
                    var middle = (MiddlewareTransport)transport;
                    middle.inner.SetPort(port);
                }
            },
            { typeof(LatencySimulation), (transport, port) => {
                    var latencySimulation = (LatencySimulation)transport;
                    latencySimulation.wrap.SetPort(port);
                }
            }
        };
    }

    public static bool HasGetAndSet(Type type) => getPortByType.ContainsKey(type);
    public static int GetPort(Transport transport, Type type) => getPortByType[type].Invoke(transport);
    public static void SetPort(Transport transport, Type type, int port) => setPortByType[type].Invoke(transport, port);
    public static bool HasGetAndSet(this Transport transport) => HasGetAndSet(transport.GetType());
    public static int GetPort(this Transport transport) => GetPort(transport, transport.GetType());
    public static void SetPort(this Transport transport, int port) => SetPort(transport, transport.GetType(), port);
}
