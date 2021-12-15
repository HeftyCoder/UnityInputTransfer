using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
using System;
using TMPro;
using UnityEngine.InputSystem.LowLevel;
using System.IO;
using UnityEngine.InputSystem;

public class HololensPhoneServer : MonoBehaviour
{
    public static HololensPhoneServer Instance;

    [SerializeField] int port = 5000;
    [SerializeField] bool onStart = false;
    
    private bool listening = false;
    private int count = 0;    
    private Queue<PhoneInputSerialization> queuedInputs = new Queue<PhoneInputSerialization>();

    public event Action<PhoneInputSerialization> onPhoneInput;
    public IServerSocket ServerSocket { get; private set; }

    private InputEventTrace eventTrace = new InputEventTrace();
    private MemoryStream stream = new MemoryStream();
    private Gamepad gp;
    private void Awake()
    {
        gp = InputSystem.AddDevice<Gamepad>("Phone Gamepad");
        InputSystem.EnableDevice(gp);

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
        InputSystem.RemoveDevice(gp);
    }

    private void InitializeServer()
    {
        ServerSocket = new ServerSocketWs();
        ServerSocket.Connected += (peer) =>
        {
            count++;
            Debug.Log($"Connected :{count}");

            peer.MessageReceived += (message) =>
            {
                var bytes = message.AsBytes();
                var stream = new MemoryStream(bytes);
                var eventTrace = new InputEventTrace();
                //stream.Write(bytes, 0, bytes.Length);
                eventTrace.ReadFrom(stream);
                var controller = eventTrace.Replay();
                int id = 0;
                foreach (var d in eventTrace.deviceInfos)
                    id = d.deviceId;
                Debug.Log($"{id} {gp.deviceId}");
                Debug.Log("xD");
                controller.WithDeviceMappedFromTo(id, gp.deviceId);
                controller.PlayAllEvents();
                
                controller.Dispose();
                eventTrace.Dispose();
                //stream.Clear();
            };
        };

        ServerSocket.Disconnected += (peer) =>
        {
            count--;
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
