using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using Barebones.Networking;

public class Test : MonoBehaviour
{
    public int maxMessages = 5;
    private IServerSocket server;
    private IClientSocket client;

    [ContextMenu("Connect")]
    private void Connect()
    {
        client.Connect("127.0.0.1", 5000);
    }
    [ContextMenu("Disconnect")]
    private void Disconnect()
    {
        client.Disconnect();
    }
    private void Awake()
    {
        server = new TelepathyServerSocket(maxMessages);
        server.Connected += (peer) =>
        {
            peer.MessageReceived += (message) =>
            {
                AttitudeInput input = new AttitudeInput();
                message.Deserialize(input);
            };
        };
        server.Disconnected += (peer) => Debug.Log(peer.Id);
        
        client = new TelepathyClientSocket(maxMessages);

        client.Connected += () => Debug.Log("Connected!");
        client.Disconnected += () => Debug.Log("Disconnected!");
    }

    private void Start()
    {
        var port = 5000;
        server.Listen(5000);
        client.Connect("127.0.0.1", port);
    }

    private void Update()
    {
        if (!client.IsConnected)
            return;
        var input = new AttitudeInput();
        var x = UnityEngine.Random.Range(0, 100);
        input.value = Quaternion.Euler(new Vector3(x, x, x));
        client.SendMessage(1, input);
    }
}
