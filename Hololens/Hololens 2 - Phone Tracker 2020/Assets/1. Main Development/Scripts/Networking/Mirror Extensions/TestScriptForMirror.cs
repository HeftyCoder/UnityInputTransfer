using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
public class TestScriptForMirror : MonoBehaviour
{
    [SerializeField] TransportClient client;
    [SerializeField] TransportServer server;

    //IClientSocket client;
    //IServerSocket server;
    IPeer peer;
    private void Start()
    {
        client.SetHandler(0, HandleSomething);
        client.Connected += () => Debug.Log("I connected");
        server.Connected += (peer) =>
        {
            Debug.Log($"I got a connection {peer.Id}");
            this.peer = peer;
        };
        client.Disconnected += () => Debug.Log("disconnected");
        server.Listen(7666);
        client.Connect("127.0.0.1", 7666);
    }

    private void OnDestroy()
    {
        client.Disconnect();
        server.Stop();
    }

    [ContextMenu("Test")]
    private void Test()
    {
        Debug.Log($"Sent {Time.frameCount}");
        peer.SendMessage(0, 242);
    }
    private void HandleSomething(IIncommingMessage message)
    {
        Debug.Log($"Received {Time.frameCount}");
    }
}
