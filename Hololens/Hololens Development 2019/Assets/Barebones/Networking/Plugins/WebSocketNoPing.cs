using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Barebones.Networking
{
    public class WebSocketNoPing : WebSocket
    {
        public WebSocketNoPing(Uri url) : base(url) { }

        public override IEnumerator Connect()
        {
            m_Socket = new WebSocketSharp.WebSocket(mUrl.ToString());
            m_Socket.OnMessage += (sender, e) =>
            {
                m_Messages.Enqueue(e.RawData);
            };
            m_Socket.OnOpen += (sender, e) =>
            {
                m_IsConnected = true;
            };
            m_Socket.OnError += (sender, e) =>
            {
                m_Error = e.Message;
                Logs.Error(e.Message);
            };
            m_Socket.OnClose += (sender, args) => m_IsConnected = false;

            if (SupportsThreads)
            {
                ThreadPool.QueueUserWorkItem((status) =>
                {
                    m_Socket.Connect();
                });
            }
            else
            {
                m_Socket.Connect();
            }

            IsConnecting = true;
            while (!m_IsConnected && m_Error == null)
            {
                yield return null;
            }
            IsConnecting = false;
        }
    }
}

