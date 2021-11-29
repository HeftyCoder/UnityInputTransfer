using System;
using System.Collections.Generic;
using UnityEngine;
using Hefty.WebSocketServer;
using Barebones.Networking;
using TMPro;
public class Test : MonoBehaviour
{
    [SerializeField] string ip = "127.0.0.1";
    [SerializeField] int port = 5000;
    [SerializeField] TMP_Text status;
    private IServerSocket serverSocket = new ServerSocketWs();
    private IClientSocket clientSocket = new ClientSocketWs();
    
    private void Awake()
    {
        clientSocket.StatusChanged += (status) => this.status.text = status.ToString();
    }
    void Start()
    {
        serverSocket.Listen(port);
        clientSocket.Connect(ip, port);
    }

    private void OnDestroy()
    {
        clientSocket.Disconnect();
        serverSocket.Stop();
    }
}
