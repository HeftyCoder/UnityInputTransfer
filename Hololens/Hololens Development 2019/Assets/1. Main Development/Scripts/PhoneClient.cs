using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Barebones.Networking;
using TMPro;
using UnityEngine.InputSystem;

public class PhoneClient : MonoBehaviour
{
    [SerializeField] bool connectOnStart;
    [SerializeField] string ip = "127.0.0.1";
    [SerializeField] int port = 5000;
    [SerializeField] PhoneSerializer inputSerializer;
    [SerializeField] float intervalMills = 50;

    private float currentTime = 0;
    bool startMessaging = false;
    public IClientSocket ClientSocket { get; private set; } = new ClientSocketWs();
    public PhoneSerializer InputSerializer => inputSerializer;

    private void Awake()
    {
        ClientSocket.Connected += () =>
        {
            var deviceDatas = new List<DeviceData>();
            foreach (var device in InputSystem.devices)
            {
                var deviceData = new DeviceData(device, InputDeviceChange.Added);
                deviceDatas.Add(deviceData);
            }

            var phoneData = new PhoneData(deviceDatas, new byte[0]);

            ClientSocket.SendMessage((short)NetworkingCodes.PhoneInput, phoneData);
            startMessaging = true;
        };

        ClientSocket.Disconnected += () => startMessaging = false;
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

        if (currentTime < intervalMills)
        {
            currentTime += Time.deltaTime * 1000;
            return;
        }

        currentTime = 0;
        var phoneData = inputSerializer.GetPhoneData();
        ClientSocket.SendMessage((short)NetworkingCodes.PhoneInput, phoneData);

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

    public void Disconnect() => ClientSocket.Disconnect();
}
