using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Barebones.Networking;

public class TransportServer : BaseTransportSocket, IServerSocket
{
    private class TransportServerPeer : BasePeer
    {
        public int id;
        public bool isConnected = false;
        public Transport transport;
        public TransportServerPeer(Transport transport, int id)
        {
            this.transport = transport;
            this.id = id;
        }
        public override int Id => id;
        public override bool IsConnected => isConnected;

        public override void Disconnect(string reason)
        {
            isConnected = false;
            transport.ServerDisconnect(id);
        }

        public override void SendMessage(IMessage message, DeliveryMethod deliveryMethod)
        {
            var bytes = message.ToBytes();
            var segment = new ArraySegment<byte>(bytes);
            var channel = GetChannelId(deliveryMethod);
            transport.ServerSend(id, segment, channel);
        }
    }
    public event PeerActionHandler Connected;
    public event PeerActionHandler OnConnected;
    public event PeerActionHandler Disconnected;
    public event PeerActionHandler OnDisconnected;

    private Dictionary<int, BasePeer> peers = new Dictionary<int, BasePeer>();
    private void Awake()
    {
        if (transport == null)
            transport = GetComponent<Transport>();
        void handleConnect(int id)
        {
            var peer = new TransportServerPeer(transport, id);
            peers.Add(id, peer);
            Connected?.Invoke(peer);
        }
        transport.OnServerConnected = handleConnect;

        void handleDisconnect(int id)
        {
            peers.TryGetValue(id, out var peer);
            peers.Remove(id);
            Disconnected?.Invoke(peer);
        }
        transport.OnServerDisconnected = handleDisconnect;

        void handleData(int id, ArraySegment<byte> data, int channel)
        {
            if (!peers.TryGetValue(id, out BasePeer peer))
                return;
            var myData = new byte[data.Count];
            var fullData = data.Array;
            for (int i = 0; i < myData.Length; i++)
            {
                myData[i] = fullData[data.Offset + i];
            }
            peer.HandleDataReceived(myData, 0);
        }

        transport.OnServerDataReceived = handleData;
        NetworkTransportLoop.AddServer(this);
    }
    private void OnDestroy()
    {
        NetworkTransportLoop.RemoveServer(this);
    }

    public void Listen()
    {
        transport.ServerStart();
    }
    public void Listen(int port)
    {
        transport.SetPort(port);
        transport.ServerStart();
    }

    public void Stop()
    {
        transport.ServerStop();
    }
}
