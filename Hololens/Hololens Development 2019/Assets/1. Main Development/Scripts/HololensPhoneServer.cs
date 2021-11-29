using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
using System;
using TMPro;

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
        if (onStart)
            Listen();
    }
    //Not afraid of concurrency because we're polling every frame for an update
    //Either way, our sockets invoke the events on Unity's Main Thread
    private void FixedUpdate()
    {
        if (queuedInputs.Count == 0)
            return;
        var input = queuedInputs.Dequeue();
        onPhoneInput?.Invoke(input);
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
                var input = new PhoneInputSerialization();
                message.Deserialize(input);
                queuedInputs.Enqueue(input);
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
