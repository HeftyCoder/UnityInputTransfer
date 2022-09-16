using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
using System;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
public class PhoneServer : MonoBehaviour
{
    public static PhoneServer Instance;

    [SerializeField] TransportServer server;
    [SerializeField] int targetFramerate = -1;
    [SerializeField] int port = 5000;
    [SerializeField] bool onStart = false;
    [SerializeField] float delayThreshold = 1f;
    [SerializeField] ArucoTracker arucoTracker;
    [SerializeField] bool enableArucoTrackerOnConnection = false;

    private bool listening = false;
    private int count = 0;
    private float timeSinceLastMessage = 0;
    private PhoneClient localClient;
    public IServerSocket ServerSocket => server;

    private InputActions inputActions;
    private InputEventTrace eventTrace = new InputEventTrace();
    private Dictionary<IPeer, Dictionary<string, InputDevice>> peerToDevices = new Dictionary<IPeer, Dictionary<string,InputDevice>>();

    public InputActions InputActions => inputActions;
    public HashSet<InputDevice> CreatedDevices { get; private set; } = new HashSet<InputDevice>();
    public bool haveDevicesChanged = false;
    public Dictionary<short, Action<IIncommingMessage>> Operations { get; private set; } = new Dictionary<short, Action<IIncommingMessage>>();
    private void Awake()
    {
        inputActions = new InputActions();
        RefreshDevices();
        localClient = GetComponent<PhoneClient>();
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(gameObject);
        
        Instance = this;
    }
    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();
    private void Start()
    {
        Application.targetFrameRate = targetFramerate;
        InitializeServer();
        if (onStart)
            Listen();
    }

    private void OnDestroy()
    {
        foreach (var device in CreatedDevices)
            InputSystem.RemoveDevice(device);
    }

    private void Update()
    {
        timeSinceLastMessage += Time.deltaTime;
        Debug.Log("SERVER: "+Mirror.NetworkTime.time);
    }
    private void InitializeServer()
    {
        Operations.Add((short)global::Operations.Subscribe, this.OnSubscribe);
        Operations.Add((short)global::Operations.StateData, this.OnPhoneData);

        ServerSocket.Connected += (peer) =>
        {
            count++;
            var serverDevices = new Dictionary<string, InputDevice>();
            peerToDevices.Add(peer, serverDevices);
            Debug.Log($"Connected :{count}");

            peer.MessageReceived += (message) =>
            {
                //if (timeSinceLastMessage > delayThreshold)
                //    Debug.Log($"Message Processing took: {timeSinceLastMessage}");
                timeSinceLastMessage = 0;
                var opcode = message.OpCode;
                Operations[opcode].Invoke(message);
            };
        };

        ServerSocket.Disconnected += (peer) =>
        {
            count--;
            Debug.Log($"Diconnected: {count}");
            Clear(peer);
        };
    }
    public void EnableTracker(bool enable) => arucoTracker.enabled = enable;
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
 
        arucoTracker.boardPositions.arucoLayout = subData.arucoLayout;
        
        if (enableArucoTrackerOnConnection)
            arucoTracker.enabled = true;

        var peer = message.Peer;
        foreach (var desc in subData.devices)
        {
            AddDevice(desc, peer);
        }

        RefreshDevices();
    }
    private void OnPhoneData(IIncommingMessage message)
    {
        var phoneData = new PhoneData();
        message.Deserialize(phoneData);
        var peer = message.Peer;

        //datas[peer] = phoneData;
        ProcessPhoneData(peer, phoneData);
    }

    private void ProcessPhoneData(IPeer peer, PhoneData phoneData)
    {
        foreach (var data in phoneData.inputDatas)
        {
            var desc = data.deviceDescription;
            switch (data.deviceChange)
            {
                case InputDeviceChange.Added:
                    haveDevicesChanged = true;
                    var existingDevice = GetDevice(desc.Layout, peer);
                    if (existingDevice != null)
                    {
                        Debug.Log($"There's already a device with {desc.Layout}");
                        return;
                    }
                    Debug.Log(desc.device);
                    AddDevice(desc, peer);
                    break;
                case InputDeviceChange.Removed:
                    haveDevicesChanged = true;
                    RemoveDevice(data.deviceDescription, peer);
                    continue;
            }

            var input = data.inputData;
            var device = GetDevice(desc.Layout, peer);
            input.QueueInput(device);
        }
        if (haveDevicesChanged)
            RefreshDevices();

        foreach (var ev in phoneData.events)
        {
            var message = MessageHelper.FromBytes(ev.bytes, 0, peer);
            var opcode = message.OpCode;
            if (Operations.TryGetValue(opcode, out var result))
                result?.Invoke(message);
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
        Debug.Log($"{name} {device}");
        CreatedDevices.Add(device);
    }
    private void RemoveDevice(DeviceDescription desc, IPeer peer)
    {
        localClient?.SetCaptureEvents(false);

        var layout = desc.Layout;
        var peerDevices = peerToDevices[peer];
        if (!peerDevices.TryGetValue(layout, out var device))
            return;
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
        haveDevicesChanged = true;
        localClient?.SetCaptureEvents(false);
        
        foreach (var device in peerToDevices[peer].Values)
        {
            CreatedDevices.Remove(device);
            InputSystem.RemoveDevice(device);
        }

        peerToDevices.Remove(peer);
        localClient?.SetCaptureEvents(true);
        RefreshDevices();
    }

    private void RefreshDevices()
    {
        var createdDevices = CreatedDevices;
        var devices = new InputDevice[createdDevices.Count];

        int i = 0;
        foreach (var device in createdDevices)
        {
            devices[i] = device;
            i++;
        }

        inputActions.devices = new ReadOnlyArray<InputDevice>(devices);
    }
    #endregion
    
}
