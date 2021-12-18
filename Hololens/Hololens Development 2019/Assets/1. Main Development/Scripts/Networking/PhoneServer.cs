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

    private PhoneClient localClient;

    private void Awake()
    {
        localClient = GetComponent<PhoneClient>();
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
            Clear(peer);
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
        foreach (var desc in subData.devices)
        {
            Debug.Log(desc.device);
            AddDevice(desc, peer);
        }
    }
    private void OnPhoneData(IIncommingMessage message)
    {;
        var phoneData = new PhoneData();
        message.Deserialize(phoneData);
        var peer = message.Peer;

        foreach (var data in phoneData.inputDatas)
        {
            var desc = data.deviceDescription;
            switch (data.deviceChange)
            {
                case InputDeviceChange.Added:
                    Debug.Log(desc.device);
                    AddDevice(desc, peer);
                    break;
                case InputDeviceChange.Removed:
                    RemoveDevice(data.deviceDescription, peer);
                    continue;
            }

            var input = data.inputData;
            var device = GetDevice(desc.Layout, peer);
            input.QueueInput(device);
        }
    }
    #endregion

    #region Adding-Removing Devices
    private void AddDevice(DeviceDescription desc, IPeer peer)
    {
        var name = $"{desc.CustomName}_{peer.Id}";
        var layout = desc.Layout;
        localClient?.SetCaptureEvents(false);
        var device = InputSystem.AddDevice(layout, name);
        localClient?.SetCaptureEvents(true);
        var peerDevices = peerToDevices[peer];
        peerDevices.Add(layout, device);
        CreatedDevices.Add(device);
    }
    private void RemoveDevice(DeviceDescription desc, IPeer peer)
    {
        localClient?.SetCaptureEvents(false);

        var layout = desc.Layout;
        var peerDevices = peerToDevices[peer];
        var device = peerDevices[layout];
        peerDevices.Remove(layout);
        CreatedDevices.Remove(device);
        InputSystem.RemoveDevice(device);

        localClient?.SetCaptureEvents(true);
    }

    private InputDevice GetDevice(string layout, IPeer peer)
    {
        var peerDevices = peerToDevices[peer];
        if (peerDevices.TryGetValue(layout, out InputDevice device))
            return device;
        return null;
    }

    private void Clear(IPeer peer)
    {
        localClient?.SetCaptureEvents(false);
        
        foreach (var device in peerToDevices[peer].Values)
        {
            CreatedDevices.Remove(device);
            InputSystem.RemoveDevice(device);
        }

        peerToDevices.Remove(peer);
        localClient?.SetCaptureEvents(true);
    }
    #endregion
    
}
