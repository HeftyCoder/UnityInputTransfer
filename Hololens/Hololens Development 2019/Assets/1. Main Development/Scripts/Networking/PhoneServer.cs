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
    private Dictionary<IPeer, Dictionary<string,InputDevice>> peerToDevices = new Dictionary<IPeer, Dictionary<string,InputDevice>>();

    public HashSet<InputDevice> CreatedDevices { get; private set; } = new HashSet<InputDevice>();

    private Dictionary<short, Action<IIncommingMessage>> operations = new Dictionary<short, Action<IIncommingMessage>>();

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
        foreach (var device in CreatedDevices)
            InputSystem.RemoveDevice(device);
    }
    private void InitializeServer()
    {
        ServerSocket = new ServerSocketWs();
        operations.Add((short)Operations.Subscribe, OnSubscribe);
        operations.Add((short)Operations.StateData, OnPhoneData);

        ServerSocket.Connected += (peer) =>
        {
            count++;
            var serverDevices = new Dictionary<string, InputDevice>();
            peerToDevices.Add(peer, serverDevices);
            Debug.Log($"Connected :{count}");

            peer.MessageReceived += (message) =>
            {
                var opcode = message.OpCode;
                operations[opcode].Invoke(message);
            };
        };

        ServerSocket.Disconnected += (peer) =>
        {
            count--;
            var devices = peerToDevices[peer];
            foreach (var device in devices.Values)
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

    #region Operations
    private void OnSubscribe(IIncommingMessage message)
    {
        var subData = new SubscriptionData();
        message.Deserialize(subData);
        var peer = message.Peer;
        foreach (var device in subData.devices)
        {
            var name = device.CustomName;
            var layout = device.Layout;
        }
        foreach (var deviceChange in data.DeviceChanges)
        {
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
    }
    private void OnPhoneData(IIncommingMessage message)
    {

    }
    #endregion

    #region Adding-Removing Devices
    private void AddDevice(DeviceDescription desc, IPeer peer)
    {
        var name = $"{desc.customName}_{peer.Id}";
        var layout = desc.Layout;
        var device = InputSystem.AddDevice(layout, name);
        var peerDevices = peerToDevices[peer];
        peerDevices.Add(layout, device);
        CreatedDevices.Add(device);
    }
    private void AddDevice(string layout, InputDevice device, Dictionary<string,InputDevice> devices)
    {
        devices.Add(layout, device);
        CreatedDevices.Add(device);
    }
    private void RemoveDevice(string layout, InputDevice device, Dictionary<string, InputDevice> devices)
    {
        devices.Remove(layout);
        RemoveDevice(device);
    }
    private void RemoveDevice(InputDevice device)
    {
        //Order is important
        InputSystem.RemoveDevice(device);
        CreatedDevices.Remove(device);
    }
    #endregion
    
}
