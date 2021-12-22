using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kcp2k;
using Barebones.Networking;
using Telepathy;
using System;

namespace Barebones.Networking
{
    public class TelepathyServerSocket : IServerSocket, IUpdatable
    {
        #region Telepathy Server Peer;
        private class TelepathyServerPeer : BasePeer
        {
            private Server telepathyServer;
            public int id;
            public bool isConnected = false;
            public TelepathyServerPeer(int id, Server server)
            {
                telepathyServer = server;
                this.id = id;
                isConnected = true;
            }
            public override int Id => id;
            public override bool IsConnected => isConnected;

            public override void Disconnect(string reason)
            {
                telepathyServer.Disconnect(id);
                isConnected = false;
            }

            public override void SendMessage(IMessage message, DeliveryMethod deliveryMethod)
            {
                var bytes = message.ToBytes();
                var segment = new ArraySegment<byte>(bytes);
                telepathyServer.Send(id, segment);
            }
        }
        #endregion

        public event PeerActionHandler Connected;
        public event PeerActionHandler Disconnected;

        //Useless
        public event PeerActionHandler OnConnected;
        public event PeerActionHandler OnDisconnected;

        Telepathy.Server server;
        Dictionary<int, BasePeer> peers = new Dictionary<int, BasePeer>();
        int maxMessageSize = 5;
        public TelepathyServerSocket(int maxMessageSize)
        {
            server = new Server(maxMessageSize);
            this.maxMessageSize = maxMessageSize;
            server.OnConnected += (id) =>
            {
                var peer = new TelepathyServerPeer(id, server);
                peers.Add(id, peer);
                Connected?.Invoke(peer);
            };

            server.OnDisconnected += (id) =>
            {
                if (!peers.TryGetValue(id, out BasePeer peer))
                    return;
                peer.Disconnect("");
                Disconnected?.Invoke(peer);
            };

            server.OnData += (int id, ArraySegment<byte> data) =>
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
            };
        }
        public void Listen(int port)
        {
            server.Start(port);
            BTimer.Instance.ApplicationQuit += Stop;
            BmUpdateRunner.Instance.Add(this);
        }

        public void Stop() => server.Stop();

        public void Update()
        {
            server.Tick(maxMessageSize);
        }
    }
}

