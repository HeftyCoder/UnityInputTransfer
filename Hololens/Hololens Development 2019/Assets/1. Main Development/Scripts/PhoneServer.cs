using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
using System;
using TMPro;
using UnityEngine.InputSystem.LowLevel;
using System.IO;
using UnityEngine.InputSystem;

public class PhoneServer : MonoBehaviour
{
    public static PhoneServer Instance;

    [SerializeField] int port = 5000;
    [SerializeField] bool onStart = false;
    
    private bool listening = false;
    private int count = 0;    

    public IServerSocket ServerSocket { get; private set; }

    private InputEventTrace eventTrace = new InputEventTrace();
    private Dictionary<IPeer, HashSet<InputDevice>> peerToId = new Dictionary<IPeer, HashSet<InputDevice>>();

    private Dictionary<int, InputDevice> clientIdToServerDevice = new Dictionary<int, InputDevice>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(gameObject);
        
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
        foreach (var device in clientIdToServerDevice.Values)
            InputSystem.RemoveDevice(device);
    }
    private void InitializeServer()
    {
        ServerSocket = new ServerSocketWs();
        ServerSocket.Connected += (peer) =>
        {
            count++;
            var serverDevices = new HashSet<InputDevice>();
            peerToId.Add(peer, serverDevices);
            Debug.Log($"Connected :{count}");

            peer.MessageReceived += (message) =>
            {
                var data = new PhoneData();
                message.Deserialize(data);
                
                foreach (var deviceChange in data.DeviceChanges)
                {
                    switch (deviceChange.DeviceChange) 
                    {
                        case InputDeviceChange.Added:
                            var device = InputSystem.AddDevice(deviceChange.Layout, deviceChange.Name);
                            Debug.Log($"Added device: {device.name} {device.deviceId}");
                            serverDevices.Add(device);
                            clientIdToServerDevice.Add(deviceChange.Id, device);
                            break;
                        case InputDeviceChange.Removed:
                            var id = deviceChange.Id;
                            var serverDevice = clientIdToServerDevice[id];

                            serverDevices.Remove(serverDevice);
                            clientIdToServerDevice.Remove(id);
                            InputSystem.RemoveDevice(serverDevice);
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
                    Debug.Log($"{clientId}");
                    var serverId = clientIdToServerDevice[clientId].deviceId;
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
            peerToId.Remove(peer);
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

}
