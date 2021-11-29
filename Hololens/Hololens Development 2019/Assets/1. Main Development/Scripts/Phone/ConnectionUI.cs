using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Barebones.Networking;
using TMPro;
public class ConnectionUI : MonoBehaviour
{
    const string ipKey = "IP";
    const string portKey = "port";

    [SerializeField] HololensPhoneClient phoneClient;
    [SerializeField] Button connectButton;
    [SerializeField] TMP_InputField ipInput;
    [SerializeField] TMP_InputField portInput;
    [SerializeField] TMP_Text connectionStatus;

    [Header("UI Views")]
    [SerializeField] GameObject connectionUI;
    [SerializeField] GameObject controlUI;
    [SerializeField] float changeUITimeSeconds = 1.5f;
    
    private IClientSocket clientSocket => phoneClient.ClientSocket;
    private void Awake()
    {
        connectionStatus.text = "Please connect";
        connectButton.onClick.AddListener(PressConnectButton);
        var ip = PlayerPrefs.GetString(ipKey);
        if (!string.IsNullOrEmpty(ip))
            ipInput.text = ip;
        var portString = PlayerPrefs.GetString(portKey);
        if (!string.IsNullOrEmpty(portString) && int.TryParse(portString, out int port))
            ipInput.text = portString;
    }
    private void Start()
    {
        InitializeConnectionEvents();
    }
    private void InitializeConnectionEvents()
    {
        clientSocket.Connected += () =>
        {
            var ip = clientSocket.ConnectionIp;
            var port = clientSocket.ConnectionPort;
            PlayerPrefs.SetString(ipKey, ip);
            PlayerPrefs.SetString(portKey, port.ToString());

            StartCoroutine(ShowControlUI());
        };

        clientSocket.Disconnected += () =>
        {
            StartCoroutine(HideControlUI());
        };
    }
    private void PressConnectButton()
    {
        var ip = ipInput.text;
        var port = int.Parse(portInput.text);
        phoneClient.Connect(ip, port);
    }

    private IEnumerator ShowControlUI()
    {
        connectButton.interactable = false;
        phoneClient.enabled = false;
        connectionStatus.text = "Connected";
        yield return new WaitForSeconds(changeUITimeSeconds);
        connectionUI.SetActive(false);
        controlUI.SetActive(true);
        phoneClient.enabled = true;
    }
    private IEnumerator HideControlUI()
    {
        phoneClient.enabled = false;
        connectionStatus.text = "Please connect";
        yield return new WaitForSeconds(changeUITimeSeconds);
        connectionUI.SetActive(true);
        controlUI.SetActive(false);
        connectButton.interactable = true;
    }
}
