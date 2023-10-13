// our ideal update looks like this:
//   transport.process_incoming()
//   update_world()
//   transport.process_outgoing()
//

//This is taken directly from NetworkLoop of Mirror. Changed only a few things. Check their implementation
//So, check their implementation for more comments. The comments in here are deleted so it's more readable
using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
// PlayerLoop and LowLevel were in the Experimental namespace until 2019.3
// https://docs.unity3d.com/2019.2/Documentation/ScriptReference/Experimental.LowLevel.PlayerLoop.html
// https://docs.unity3d.com/2019.3/Documentation/ScriptReference/LowLevel.PlayerLoop.html
#if UNITY_2019_3_OR_NEWER
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
#else
using UnityEngine.Experimental.LowLevel;
using UnityEngine.Experimental.PlayerLoop;
#endif

public static class NetworkTransportLoop
{
    // helper enum to add loop to begin/end of subSystemList
    internal enum AddMode { Beginning, End }
    static HashSet<TransportClientSocket> clients = new HashSet<TransportClientSocket>();
    static HashSet<TransportServerSocket> servers = new HashSet<TransportServerSocket>();
    public static bool AddServer(TransportServerSocket server) => servers.Add(server);
    public static bool RemoveServer(TransportServerSocket server) => servers.Remove(server);
    public static bool AddClient(TransportClientSocket client) => clients.Add(client);
    public static bool RemoveClient(TransportClientSocket client) => clients.Remove(client);
    // callbacks in case someone needs to use early/lateupdate too.
    public static Action OnEarlyUpdate;
    public static Action OnLateUpdate;

    internal static int FindPlayerLoopEntryIndex(PlayerLoopSystem.UpdateFunction function, PlayerLoopSystem playerLoop, Type playerLoopSystemType)
    {
        // did we find the type? e.g. EarlyUpdate/PreLateUpdate/etc.
        if (playerLoop.type == playerLoopSystemType)
            return Array.FindIndex(playerLoop.subSystemList, (elem => elem.updateDelegate == function));

        // recursively keep looking
        if (playerLoop.subSystemList != null)
        {
            for (int i = 0; i < playerLoop.subSystemList.Length; ++i)
            {
                int index = FindPlayerLoopEntryIndex(function, playerLoop.subSystemList[i], playerLoopSystemType);
                if (index != -1) return index;
            }
        }
        return -1;
    }
    internal static bool AddToPlayerLoop(PlayerLoopSystem.UpdateFunction function, Type ownerType, ref PlayerLoopSystem playerLoop, Type playerLoopSystemType, AddMode addMode)
    {
        // did we find the type? e.g. EarlyUpdate/PreLateUpdate/etc.
        if (playerLoop.type == playerLoopSystemType)
        {
            // resize & expand subSystemList to fit one more entry
            int oldListLength = (playerLoop.subSystemList != null) ? playerLoop.subSystemList.Length : 0;
            Array.Resize(ref playerLoop.subSystemList, oldListLength + 1);
            PlayerLoopSystem system = new PlayerLoopSystem
            {
                type = ownerType,
                updateDelegate = function
            };

            // prepend our custom loop to the beginning
            if (addMode == AddMode.Beginning)
            {
                // shift to the right, write into first array element
                Array.Copy(playerLoop.subSystemList, 0, playerLoop.subSystemList, 1, playerLoop.subSystemList.Length - 1);
                playerLoop.subSystemList[0] = system;

            }
            // append our custom loop to the end
            else if (addMode == AddMode.End)
            {
                // simply write into last array element
                playerLoop.subSystemList[oldListLength] = system;
            }

            return true;
        }

        // recursively keep looking
        if (playerLoop.subSystemList != null)
        {
            for (int i = 0; i < playerLoop.subSystemList.Length; ++i)
            {
                if (AddToPlayerLoop(function, ownerType, ref playerLoop.subSystemList[i], playerLoopSystemType, addMode))
                    return true;
            }
        }
        return false;
    }

    // hook into Unity runtime to actually add our custom functions
    [RuntimeInitializeOnLoadMethod]
    static void RuntimeInitializeOnLoad()
    {
        PlayerLoopSystem playerLoop =
#if UNITY_2019_3_OR_NEWER
            PlayerLoop.GetCurrentPlayerLoop();
#else
            PlayerLoop.GetDefaultPlayerLoop();
#endif

        AddToPlayerLoop(NetworkEarlyUpdate, typeof(NetworkTransportLoop), ref playerLoop, typeof(EarlyUpdate), AddMode.End);

        AddToPlayerLoop(NetworkLateUpdate, typeof(NetworkTransportLoop), ref playerLoop, typeof(PreLateUpdate), AddMode.End);

        // set the new loop
        PlayerLoop.SetPlayerLoop(playerLoop);
    }

    static void NetworkEarlyUpdate()
    {
        //Debug.Log($"NetworkEarlyUpdate {Time.time}");
        OnEarlyUpdate?.Invoke();
        foreach (var clientTransport in clients)
            clientTransport.Transport.ClientEarlyUpdate();
        foreach (var serverTransport in servers)
            serverTransport.Transport.ServerEarlyUpdate();
    }

    static void NetworkLateUpdate()
    {
        OnLateUpdate?.Invoke();
        foreach (var clientTransport in clients)
            clientTransport.Transport.ClientLateUpdate();
        foreach (var serverTransport in servers)
            serverTransport.Transport.ServerLateUpdate();
    }
}
