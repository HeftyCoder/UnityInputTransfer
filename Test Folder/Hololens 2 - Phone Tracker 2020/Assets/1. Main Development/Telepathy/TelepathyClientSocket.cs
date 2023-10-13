using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
using Telepathy;
using System;

namespace Barebones.Networking 
{
    public class TelepathyClientSocket : BaseClientSocket, IClientSocket, IUpdatable
    {
        #region Telepathy Client Peer
        private class TelepathyClientPeer : BasePeer
        {
            Client telepathyClient;
            public bool connected = false;
            public TelepathyClientPeer(Client telepathyClient)
            {
                this.telepathyClient = telepathyClient;
            }
            public override bool IsConnected => connected;

            public override void Disconnect(string reason)
            {
                telepathyClient.Disconnect();
                connected = false;
            }

            public override void SendMessage(IMessage message, DeliveryMethod deliveryMethod)
            {
                var bytes = message.ToBytes();
                var segment = new ArraySegment<byte>(bytes);
                telepathyClient.Send(segment);
            }
        }
        #endregion
        public ConnectionStatus Status { get; private set; }

        public bool IsConnected => Status == ConnectionStatus.Connected;

        public bool IsConnecting => Status == ConnectionStatus.Connecting;

        public string ConnectionIp { get; private set; }
        public int ConnectionPort { get; private set; }

        public event Action Connected;
        public event Action Disconnected;
        public event Action<ConnectionStatus> StatusChanged;

        private readonly Dictionary<short, IPacketHandler> _handlers;
        private Client telepathyClient;
        private int maxMessageSize = 5;
        private TelepathyClientPeer peer;
        public TelepathyClientSocket(int maxMessageSize)
        {
            this.maxMessageSize = maxMessageSize;
            telepathyClient = new Client(maxMessageSize);
            _handlers = new Dictionary<short, IPacketHandler>();
            telepathyClient.OnConnected += () =>
            {
                SetStatus(ConnectionStatus.Connected);
                StatusChanged?.Invoke(Status);
                peer.connected = true;
                Connected?.Invoke();
            };
            telepathyClient.OnDisconnected += () =>
            {
                SetStatus(ConnectionStatus.Disconnected);
                Disconnected?.Invoke();
                BmUpdateRunner.Instance.Remove(this);
            };
            telepathyClient.OnData += (ArraySegment<byte> data) =>
            {
                var myData = new byte[data.Count];
                var fullData = data.Array;
                for (int i = 0; i < myData.Length; i++)
                {
                    myData[i] = fullData[data.Offset + i];
                }

                peer.HandleDataReceived(myData, 0);
            };
        }

        private void SetUp()
        {
            peer = new TelepathyClientPeer(telepathyClient);
            peer.MessageReceived += HandleMessage;
            Peer = peer;
        }
        public IClientSocket Connect(string ip, int port, int timeoutMillis)
        {
            if (IsConnected)
                return this;
            telepathyClient.SendTimeout = timeoutMillis;
            telepathyClient.ReceiveTimeout = timeoutMillis;
            return Connect(ip, port);
        }

        public IClientSocket Connect(string ip, int port)
        {
            if (IsConnected)
                return this;
            SetUp();
            SetStatus(ConnectionStatus.Connecting);
            ConnectionIp = ip;
            ConnectionPort = port;
            telepathyClient.Connect(ip, port);
            BmUpdateRunner.Instance.Add(this);
            return this;
        }

        private void SetStatus(ConnectionStatus status)
        {
            Status = status;
            StatusChanged?.Invoke(status);
        }
        public void Disconnect() => telepathyClient.Disconnect();

        public void Reconnect()
        {
            Disconnect();
            Connect(ConnectionIp, ConnectionPort);
        }

        public void Update()
        {
            telepathyClient.Tick(maxMessageSize);
        }

        public void WaitConnection(Action<IClientSocket> connectionCallback, float timeoutSeconds)
        {
        }

        public void WaitConnection(Action<IClientSocket> connectionCallback)
        {
        }

        #region Copy-Paste Handlers
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
            _handlers[handler.OpCode] = handler;
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
            _handlers.TryGetValue(handler.OpCode, out previousHandler);

            if (previousHandler != handler)
                return;

            _handlers.Remove(handler.OpCode);
        }

        private void HandleMessage(IIncommingMessage message)
        {
            try
            {
                IPacketHandler handler;
                _handlers.TryGetValue(message.OpCode, out handler);

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
        #endregion
    }
}


