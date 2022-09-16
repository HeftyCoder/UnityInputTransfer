using Barebones.Networking;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TMPro;
using UnityEngine.InputSystem.XR;

public class PhoneClient : MonoBehaviour
{
    [SerializeField] TransportClient client;
    [SerializeField] MarkersLayout arucoMarkersLayout;
    [SerializeField] bool connectOnStart;
    [SerializeField] string ip = "127.0.0.1";
    [SerializeField] int port = 5000;
    [SerializeField] int targetFrameRate = -1;

    [Header("Devices To Connect")]
    [SerializeField]
    DeviceDescription[] sensorsToConnect;
    [SerializeField]
    DeviceDescription[] devicesToConnect;

    [Header("Motion Tracking")]
    [SerializeField] Transform trackedMotion;
    [SerializeField] string deviceName;

    TrackedPoseDriver poseDriver;
    DeviceDescription trackedDeviceDescription;
    private bool captureChangeEvents = true;
    //Care not to use duplicates above
    private Dictionary<string, DeviceDescription> layoutToDescription = new Dictionary<string, DeviceDescription>();
    private Dictionary<string, InputData> gatheredData = new Dictionary<string, InputData>();
    
    IEnumerable<DeviceDescription> internalDeviceDescriptionQuery
    {
        get
        {
            foreach (var desc in sensorsToConnect)
            {
                var device = InputSystemExtensions.GetDeviceAssignable(desc.Layout);
                if (device != null)
                    yield return desc;
            }
                
            foreach (var desc in devicesToConnect)
            {
                var device = InputSystemExtensions.GetDeviceAssignable(desc.Layout);
                if (device != null)
                    yield return desc;
            }

            //Virtually creating a tracked device to send to the server
            if (trackedDeviceDescription != null)
            {
                yield return trackedDeviceDescription;
            }
        }
    }

    //To avoid conflict in case this is running locally
    PhoneServer localServer;
    bool startMessaging = false;

    public List<EventPacket> Events { get; private set; } = new List<EventPacket>();
    public IEnumerable<DeviceDescription> DeviceDescriptions => internalDeviceDescriptionQuery;
    public IClientSocket ClientSocket => client;
    
    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = targetFrameRate;
        if (trackedMotion != null)
        {
            trackedDeviceDescription = new DeviceDescription();
            trackedDeviceDescription.customName = deviceName;
            trackedDeviceDescription.device = $"<{nameof(TrackedDevice)}>";
            trackedDeviceDescription.deviceId = -99;
        }
        //
        localServer = GetComponent<PhoneServer>();
        //Add all the needed layouts
        foreach (var desc in sensorsToConnect)
            layoutToDescription.Add(desc.Layout, desc);
        foreach (var desc in devicesToConnect)
            layoutToDescription.Add(desc.Layout, desc);

        ClientSocket.Connected += () =>
        {
            var data = new SubscriptionData(DeviceDescriptions);
            if (arucoMarkersLayout != null)
                data.arucoLayout = arucoMarkersLayout.CalculateBoard();

            Debug.Log("Sending subscribe");
            ClientSocket.SendMessage((short)Operations.Subscribe, data);
           
            startMessaging = true;
            //Enable sensors here
            foreach (var sensorDesc in sensorsToConnect)
            {
                var device = InputSystemExtensions.GetDeviceAssignable(sensorDesc.Layout);
                if (device != null)
                    InputSystem.EnableDevice(device);
            }
            InputSystem.onDeviceChange += OnDeviceChange;
            InputState.onChange += OnStateChange;
        };

        ClientSocket.Disconnected += () =>
        {
            Debug.Log("Disconnected");
            gatheredData.Clear();
            startMessaging = false;
            InputSystem.onDeviceChange -= OnDeviceChange;
            InputState.onChange -= OnStateChange;
            //Disable sensors
            foreach (var sensorDesc in sensorsToConnect)
            {
                var device = InputSystemExtensions.GetDeviceAssignable(sensorDesc.Layout);
                if (device != null)
                    InputSystem.DisableDevice(device);
            }
        };
    }

    private void Start()
    {
        if (connectOnStart)
            Connect();
    }
    private void Update()
    {
        if (!ClientSocket.IsConnected || !startMessaging)
            return;

        Debug.Log($"CLIENT: {Mirror.NetworkTime.time}");
        if (Events.Count != 0 || gatheredData.Count != 0 || trackedMotion != null)
        {
            PhoneData data = new PhoneData(gatheredData.Values, Events);
            if (trackedMotion != null)
            {
                var motionInput = new InputData(trackedDeviceDescription);

                var position = trackedMotion.localPosition;
                var rotation = trackedMotion.localRotation;
                motionInput.inputData = new TrackedDeviceInput(position, rotation);
                data.inputDatas.Add(motionInput);
            }

            ClientSocket.SendMessage((short)Operations.StateData, data);
            gatheredData.Clear();
            Events.Clear();
        }
    }
    private void OnDestroy()
    {
        ClientSocket.Disconnect();
    }
    private void OnApplicationQuit()
    {
        ClientSocket.Disconnect();
    }

    public void Connect()
    {
        Connect(ip, port);
    }
    public void Connect(string ip, int port)
    {
        if (ClientSocket.IsConnected)
            return;
        ClientSocket.Connect(ip, port);
    }

    public void Disconnect() => ClientSocket.Disconnect();

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change) 
        {
            case InputDeviceChange.Added:
                break;
            case InputDeviceChange.Removed:
                break;
            default:
                return;
        }

        if (IsDeviceFromLocalServer(device))
            return;
        var layout = device.GetConnectingDeviceLayout();
        if (string.IsNullOrEmpty(layout) || !layoutToDescription.ContainsKey(layout))
            return;
        
        var data = GetInputData(layout);
        data.deviceChange = change;
    }

    private void OnStateChange(InputDevice device, InputEventPtr ptr)
    {
        if (IsDeviceFromLocalServer(device))
            return;

        var layout = device.GetConnectingDeviceLayout();
        if (string.IsNullOrEmpty(layout) || !layoutToDescription.ContainsKey(layout))
            return;

        var data = GetInputData(layout);
        var inputData = InputFactory.CreateInput(device, layout, ptr);
        data.inputData = inputData;
    }

    private InputData GetInputData(string layout)
    {
        if (!gatheredData.TryGetValue(layout, out InputData data))
        {
            var desc = layoutToDescription[layout];
            data = new InputData() { deviceDescription = desc};
            gatheredData.Add(layout, data);
        }
        return data;
    }
    private bool IsDeviceFromLocalServer(InputDevice device)
    {
        return !captureChangeEvents || (localServer != null && localServer.CreatedDevices.Contains(device));
    }

    public void SetCaptureEvents(bool value) => captureChangeEvents = value;

}
