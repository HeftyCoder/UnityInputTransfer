using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
using System;
using TMPro;
using UnityEngine.InputSystem.LowLevel;
using System.IO;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

public class PhoneServer : MonoBehaviour
{
    public static PhoneServer Instance;

    [SerializeField] int port = 5000;
    [SerializeField] bool onStart = false;
    
    private bool listening = false;
    private int count = 0;    

    public IServerSocket ServerSocket { get; private set; }

    private InputEventTrace eventTrace = new InputEventTrace();
    private Dictionary<IPeer, HashSet<InputDevice>> peerToDevices = new Dictionary<IPeer, HashSet<InputDevice>>();

    private Dictionary<int, InputDevice> clientToServerMap = new Dictionary<int, InputDevice>();
    private Dictionary<InputDevice, int> serverToClientMap = new Dictionary<InputDevice, int>();

    public HashSet<InputDevice> CreatedDevices { get; private set; } = new HashSet<InputDevice>();

    private PhoneClient localClient;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(gameObject);

        localClient = GetComponent<PhoneClient>();
        InitializeServer();
        Instance = this;
    }
    private void Start()
    {
        if (onStart)
            Listen();
    }

    private void OnDestroy()
    {
        foreach (var device in clientToServerMap.Values)
            InputSystem.RemoveDevice(device);
        clientToServerMap.Clear();
        serverToClientMap.Clear();
    }
    private void InitializeServer()
    {
        ServerSocket = new ServerSocketWs();
        ServerSocket.Connected += (peer) =>
        {
            count++;
            var serverDevices = new HashSet<InputDevice>();
            peerToDevices.Add(peer, serverDevices);
            Debug.Log($"Connected :{count}");

            peer.MessageReceived += (message) =>
            {
                var data = new PhoneData();
                message.Deserialize(data);
               
                foreach (var deviceChange in data.DeviceChanges)
                {
                    //Debug.Log($"{deviceChange.Name} {deviceChange.Id} {deviceChange.DeviceChange}");
                    
                    //If using client and server on the same PC, this will result in indefinetely adding devices.
                    //To avoid this, Phone Serializer and Phone Server must be on the same GameObjects in order to remove
                    //device change events from client
                    switch (deviceChange.DeviceChange) 
                    {
                        case InputDeviceChange.Added:
                            if (clientToServerMap.ContainsKey(deviceChange.Id))
                                break;
                            var device = InputSystem.AddDevice(deviceChange.Layout, deviceChange.Name);
                            AddDevice(deviceChange.Id, device, serverDevices);
                            //Remove unwanted change events
                            localClient?.InputSerializer?.ResolveLocalServerDeviceConflict(device);
                            break;
                        case InputDeviceChange.Removed:
                            var id = deviceChange.Id;
                            //Protection from local server-client
                            if (!clientToServerMap.TryGetValue(id, out InputDevice serverDevice))
                                return;
                            
                            RemoveDevice(serverDevice, serverDevices);
                            break;
                    }
                }

                var bytes = data.InputEvents;
                if (bytes.Length == 0)
                    return;

                var stream = new MemoryStream(bytes);
                var eventTrace = new InputEventTrace();
                eventTrace.ReadFrom(stream);
                
                var controller = eventTrace.Replay();
                foreach (var info in eventTrace.deviceInfos)
                {
                    var clientId = info.deviceId;
                    //Debug.Log($"{info.deviceId} {info.layout}");
                    if (!clientToServerMap.TryGetValue(clientId, out InputDevice serverDevice))
                    {
                        Debug.LogWarning($"A device of id:{clientId} was not found. You're probably runnig server and client in the same device");
                        return;
                    }
                    var serverId = clientToServerMap[clientId].deviceId;
                    controller.WithDeviceMappedFromTo(clientId, serverId);
                }
                controller.PlayAllEvents();

                controller.Dispose();
                eventTrace.Dispose();
            };
        };

        ServerSocket.Disconnected += (peer) =>
        {
            count--;
            var devices = peerToDevices[peer];
            foreach (var device in devices)
                RemoveDevice(device);
            peerToDevices.Remove(peer);
            Debug.Log($"Disconnected: {count}");
        };
    }
    
    public void Listen()
    {
        Listen(port);
    }

    public void Listen(int port)
    {
        if (listening)
            return;
        listening = true;
        ServerSocket.Listen(port);
    }

    private void AddDevice(int clientDeviceId, InputDevice device, HashSet<InputDevice> devices)
    {
        clientToServerMap.Add(clientDeviceId, device);
        serverToClientMap.Add(device, clientDeviceId);
        devices.Add(device);
        CreatedDevices.Add(device);
    }
    private void RemoveDevice(InputDevice device, HashSet<InputDevice> devices)
    {
        RemoveDevice(device);
        devices.Remove(device);
    }
    private void RemoveDevice(InputDevice device)
    {
        var clientDeviceId = serverToClientMap[device];
        serverToClientMap.Remove(device);
        clientToServerMap.Remove(clientDeviceId);
        CreatedDevices.Remove(device);
        InputSystem.RemoveDevice(device);
    }
}
