using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Barebones.Networking;
using TMPro;

public class HololensPhoneClient : MonoBehaviour
{
    [SerializeField] bool connectOnStart;
    [SerializeField] string ip = "127.0.0.1";
    [SerializeField] int port = 5000;
    [SerializeField] PhoneInputSerializer inputSerializer;
    [SerializeField] float intervalMills = 50;

    private float currentTime = 0;

    public IClientSocket ClientSocket { get; private set; } = new ClientSocketWs();

    private void Start()
    {
        if (connectOnStart)
            Connect();
    }
    private void Update()
    {
        if (!ClientSocket.IsConnected)
            return;

        if (currentTime < intervalMills)
        {
            currentTime += Time.deltaTime * 0.001f;
            return;
        }

        currentTime = 0;
        var serializedInput = inputSerializer.GetInputSerialization();
        ClientSocket.SendMessage((short)NetworkingCodes.PhoneInput, serializedInput);

    }
    private void OnDestroy()
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
}
