using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Barebones.Networking;
public class TransportClient : BaseTransportSocket, IClientSocket
{
    private class TransportClientPeer : BasePeer
    {
        public Transport transport;
        public bool connected = false;
        public override bool IsConnected => connected;

        public TransportClientPeer(Transport transport) => this.transport = transport;
        public override void Disconnect(string reason)
        {
            transport.ClientDisconnect();
            connected = false;
        }
        public override void SendMessage(IMessage message, DeliveryMethod deliveryMethod)
        {
            var bytes = message.ToBytes();
            var segment = new ArraySegment<byte>(bytes);
            var channelId = GetChannelId(deliveryMethod);
            transport.ClientSend(segment, channelId);
        }
    }

    [SerializeField] public string ipToConnect;
    private ConnectionStatus status;
    private BasePeer peer;
    private BaseClientSocket msgDispatcher; // bad naming
    private Dictionary<short, IPacketHandler> handlers;

    public ConnectionStatus Status
    {
        get => status;
        set
        {
            if (value == status)
                return;
            status = value;
            StatusChanged?.Invoke(status);
        }
    }

    public bool IsConnected => Status == ConnectionStatus.Connected;

    public bool IsConnecting => Status == ConnectionStatus.Connecting;

    public string ConnectionIp { get; private set; }

    public int ConnectionPort { get; private set; }

    public IPeer Peer => Peer;

    public event Action Connected
    {
        add => transport.OnClientConnected += value;
        remove => transport.OnClientConnected -= value;
    }

    public event Action Disconnected
    {
        add => transport.OnClientDisconnected += value;
        remove => transport.OnClientDisconnected -= value;
    }
    public event Action<ConnectionStatus> StatusChanged;

    private void Awake()
    {
        if (transport == null)
            transport = GetComponent<Transport>();
        Status = ConnectionStatus.None;
        handlers = new Dictionary<short, IPacketHandler>();
        void handleConnect()
        {
            Status = ConnectionStatus.Connected;
            StatusChanged?.Invoke(Status);
        }
        void handleDisconnect()
        {
            Status = ConnectionStatus.Disconnected;
            StatusChanged?.Invoke(Status);
        }
        transport.OnClientConnected += handleConnect;
        transport.OnClientDisconnected += handleDisconnect;
        peer = new TransportClientPeer(transport);
        msgDispatcher = new  BaseClientSocket();
        msgDispatcher.Peer = peer;

        void DataReceived(ArraySegment<byte> data, int channel)
        {
            var myData = new byte[data.Count];
            var fullData = data.Array;
            for (int i = 0; i < myData.Length; i++)
            {
                myData[i] = fullData[data.Offset + i];
            }

            peer.HandleDataReceived(myData, 0);
        }
        transport.OnClientDataReceived += DataReceived;

        NetworkTransportLoop.AddClient(this);
    }

    private void OnDestroy()
    {
        NetworkTransportLoop.RemoveClient(this);
    }
    public IClientSocket Connect(string ip, int port, int timeoutMillis)
    {
        if (IsConnected)
            return this;
        ipToConnect = ip;
        transport.ClientConnect(ip);
        return this;
    }

    public IClientSocket Connect(string ip, int port)
    {
        if (IsConnected)
            return this;
        ipToConnect = ip;
        transport.ClientConnect(ip);
        return this;
    }

    public void WaitConnection(Action<IClientSocket> connectionCallback, float timeoutSeconds)
    {
    }

    public void WaitConnection(Action<IClientSocket> connectionCallback)
    {
    }

    public void AddConnectionListener(Action callback, bool invokeInstantlyIfConnected = true)
    {
        Connected += callback;
        if (IsConnected && invokeInstantlyIfConnected)
            callback.Invoke();
    }

    public void RemoveConnectionListener(Action callback)
    {
        Connected -= callback;
    }

    public IPacketHandler SetHandler(IPacketHandler handler)
    {
        handlers[handler.OpCode] = handler;
        return handler;
    }

    public IPacketHandler SetHandler(short opCode, IncommingMessageHandler handlerMethod)
    {
        var handler = new PacketHandler(opCode, handlerMethod);
        SetHandler(handler);
        return handler;
    }

    public void RemoveHandler(IPacketHandler handler)
    {
        IPacketHandler previousHandler;
        handlers.TryGetValue(handler.OpCode, out previousHandler);

        if (previousHandler != handler)
            return;

        handlers.Remove(handler.OpCode);
    }

    public void Reconnect()
    {
        transport.ClientDisconnect();
        transport.ClientConnect(ipToConnect);
    }

    public void Disconnect() => transport.ClientDisconnect();

    public void SendMessage(short opCode) => msgDispatcher.SendMessage(opCode);
    public void SendMessage(short opCode, ISerializablePacket packet) => msgDispatcher.SendMessage(opCode);
    public void SendMessage(short opCode, ISerializablePacket packet, DeliveryMethod method) => msgDispatcher.SendMessage(opCode, packet, method);
    public void SendMessage(short opCode, ISerializablePacket packet, ResponseCallback responseCallback) => msgDispatcher.SendMessage(opCode, packet, responseCallback);
    public void SendMessage(short opCode, ISerializablePacket packet, ResponseCallback responseCallback, int timeoutSecs) =>
        msgDispatcher.SendMessage(opCode, packet, responseCallback, timeoutSecs);
    public void SendMessage(short opCode, ResponseCallback responseCallback) => msgDispatcher.SendMessage(opCode, responseCallback);
    public void SendMessage(short opCode, byte[] data) => msgDispatcher.SendMessage(opCode, data);
    public void SendMessage(short opCode, byte[] data, ResponseCallback responseCallback) => msgDispatcher.SendMessage(opCode, data, responseCallback);
    public void SendMessage(short opCode, byte[] data, ResponseCallback responseCallback, int timeoutSecs) =>
        msgDispatcher.SendMessage(opCode, data, responseCallback, timeoutSecs);
    public void SendMessage(short opCode, string data) => msgDispatcher.SendMessage(opCode, data);
    public void SendMessage(short opCode, string data, ResponseCallback responseCallback) =>
        msgDispatcher.SendMessage(opCode, data, responseCallback);
    public void SendMessage(short opCode, string data, ResponseCallback responseCallback, int timeoutSecs) =>
        msgDispatcher.SendMessage(opCode, data, responseCallback);
    public void SendMessage(short opCode, int data) => msgDispatcher.SendMessage(opCode, data);
    public void SendMessage(short opCode, int data, ResponseCallback responseCallback) =>
        msgDispatcher.SendMessage(opCode, data, responseCallback);
    public void SendMessage(short opCode, int data, ResponseCallback responseCallback, int timeoutSecs) =>
        msgDispatcher.SendMessage(opCode, data, responseCallback, timeoutSecs);
    public void SendMessage(IMessage message) => msgDispatcher.SendMessage(message);
    public void SendMessage(IMessage message, ResponseCallback responseCallback) => msgDispatcher.SendMessage(message, responseCallback);
    public void SendMessage(IMessage message, ResponseCallback responseCallback, int timeoutSecs) => msgDispatcher.SendMessage(message, responseCallback, timeoutSecs);
}
