using System;
// Networking libs
using System.Net;
using System.Net.Sockets;
// For creating a thread
using System.Threading;
// For List & ConcurrentQueue
using System.Collections.Generic;
using System.Collections.Concurrent;
// Unity & Unity events
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace Hefty.WebSocketServer {
    [System.Serializable]
    public class WebSocketOpenEvent : UnityEvent<WebSocketConnection> {}

    [System.Serializable]
    public class WebSocketMessageEvent : UnityEvent<WebSocketMessage> {}

    [System.Serializable]
    public class WebSocketCloseEvent : UnityEvent<WebSocketConnection> {}

    public class WebSocketServer : MonoBehaviour
    {
        // The tcpListenerThread listens for incoming WebSocket connections, then assigns the client to handler threads;
        private TcpListener tcpListener;
        private Thread tcpListenerThread;
        private TcpClient connectedTcpClient;

        public ConcurrentQueue<WebSocketMessage> messages;

        public string address = "127.0.0.1";
        public int port = 5000;
        private string status = "Listening";
        public TMP_Text connectionStatus;
        public WebSocketOpenEvent onOpen;
        public WebSocketMessageEvent onMessage;
        public WebSocketCloseEvent onClose;

        void Awake() {
            connectionStatus.text = "Listening";
            if (onMessage == null) onMessage = new WebSocketMessageEvent();
        }

        void Start() {
            messages = new ConcurrentQueue<WebSocketMessage>();
           
            tcpListenerThread = new Thread (new ThreadStart(ListenForTcpConnection));
            tcpListenerThread.IsBackground = true;
            tcpListenerThread.Start();
        }

        void Update() {
            connectionStatus.text = status;
            WebSocketMessage message;
            while (messages.TryDequeue(out message)) {
                onMessage.Invoke(message);
                this.OnMessage(message);
            }
        }

        private void ListenForTcpConnection () {
            try {
                // Create listener on <address>:<port>.
                tcpListener = new TcpListener(IPAddress.Parse(address), port);
                tcpListener.Start();
                while (true) {
                    // Accept a new client, then open a stream for reading and writing.
                    connectedTcpClient = tcpListener.AcceptTcpClient();
                    // Create a new connection
                    WebSocketConnection connection = new WebSocketConnection(connectedTcpClient, this);
                    // Establish connection
                    connection.Establish();
                    // // Start a new thread to handle the connection.
                    // Thread worker = new Thread (new ParameterizedThreadStart(HandleConnection));
                    // worker.IsBackground = true;
                    // worker.Start(connection);
                    // // Add it to the thread list. TODO: delete thread when disconnecting.
                    // workerThreads.Add(worker);
                }
            }
            catch (SocketException socketException) {
                Debug.Log("SocketException " + socketException.ToString());
            }
        }


        public virtual void OnOpen(WebSocketConnection connection) {
            status = connection.id;
        }

        public virtual void OnMessage(WebSocketMessage message) {}

        public virtual void OnClose(WebSocketConnection connection) {}

        public virtual void OnError(WebSocketConnection connection) {}

    }
}

