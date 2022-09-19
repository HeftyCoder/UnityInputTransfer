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

    [SerializeField] string ipToConnect = "localhost";

    private ConnectionStatus status;
    private TransportClientPeer peer;
    private BaseClientSocket msgDispatcher; // bad naming for the class
    private Dictionary<short, IPacketHandler> handlers = new Dictionary<short, IPacketHandler>();

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

    public event Action Connected;
    public event Action Disconnected;
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
            peer.connected = true;
            StatusChanged?.Invoke(Status);
            Connected?.Invoke();
        }
        void handleDisconnect()
        {
            peer.connected = false;
            Status = ConnectionStatus.Disconnected;
            StatusChanged?.Invoke(Status);
            Disconnected?.Invoke();
        }
        transport.OnClientConnected = handleConnect;
        transport.OnClientDisconnected = handleDisconnect;
        
        peer = new TransportClientPeer(transport);
        peer.MessageReceived += HandleMessage;

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
        transport.OnClientDataReceived = DataReceived;

        NetworkTransportLoop.AddClient(this);
    }

    private void OnDestroy()
    {
        NetworkTransportLoop.RemoveClient(this);
    }

    public IClientSocket Connect()
    {
        if (IsConnected)
            return this;
        transport.ClientConnect(EnsureIP(ipToConnect));
        return this;
    }
    public IClientSocket Connect(int port) => Connect(ipToConnect, port);
    public IClientSocket Connect(string ip, int port, int timeoutMillis)
    {
        //Do something here I guess
        return Connect(ip, port);
    }
    public IClientSocket Connect(string ip, int port)
    {
        if (IsConnected)
            return this;
        ipToConnect = EnsureIP(ip);
        transport.SetPort(port);
        transport.ClientConnect(ipToConnect);
        return this;
    }
    private string EnsureIP(string ip)
    {
        string result = ip;
        if (ip == "localhost")
            result = "127.0.0.1";
        return result;
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


    private void HandleMessage(IIncommingMessage message)
    {
        try
        {
            IPacketHandler handler;
            handlers.TryGetValue(message.OpCode, out handler);

            if (handler != null)
                handler.Handle(message);
            else if (message.IsExpectingResponse)
            {
                Logs.Error("Connection is missing a handler. OpCode: " + message.OpCode);
                message.Respond(ResponseStatus.Error);
            }
        }
        catch (Exception e)
        {

            Logs.Error("Failed to handle a message. OpCode: " + message.OpCode);
            Logs.Error(e);

            if (!message.IsExpectingResponse)
                return;

            try
            {
                message.Respond(ResponseStatus.Error);
            }
            catch (Exception exception)
            {
                Logs.Error(exception);
            }
        }
    }

    public void SendMessage(short opCode) => msgDispatcher.SendMessage(opCode);
    public void SendMessage(short opCode, ISerializablePacket packet) => msgDispatcher.SendMessage(opCode, packet);
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
        msgDispatcher.SendMessage(opCode, data, responseCallback, timeoutSecs);
    public void SendMessage(short opCode, int data) => msgDispatcher.SendMessage(opCode, data);
    public void SendMessage(short opCode, int data, ResponseCallback responseCallback) =>
        msgDispatcher.SendMessage(opCode, data, responseCallback);
    public void SendMessage(short opCode, int data, ResponseCallback responseCallback, int timeoutSecs) =>
        msgDispatcher.SendMessage(opCode, data, responseCallback, timeoutSecs);
    public void SendMessage(IMessage message) => msgDispatcher.SendMessage(message);
    public void SendMessage(IMessage message, ResponseCallback responseCallback) => msgDispatcher.SendMessage(message, responseCallback);
    public void SendMessage(IMessage message, ResponseCallback responseCallback, int timeoutSecs) => msgDispatcher.SendMessage(message, responseCallback, timeoutSecs);
}
