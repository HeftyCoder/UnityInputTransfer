using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Barebones.Networking;

public class TestController : MonoBehaviour
{
    private class TestClass : SerializablePacket {
        public Quaternion quat;

        public override void FromBinaryReader(EndianBinaryReader reader) => quat = reader.ReadQuaternion();
        public override void ToBinaryWriter(EndianBinaryWriter writer) => writer.Write(quat);
    }

    [SerializeField] int targetFrameRate = -1;
    AttitudeSensor att;
    Vector3 currentOrientation;

    private TestClass test = new TestClass();
    IClientSocket client = new ClientSocketWs();
    IServerSocket server = new ServerSocketWs();
    private float timeSinceLast = 0;
    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
        att = InputSystem.AddDevice<AttitudeSensor>();
    }
    private void Start()
    {
        server.Listen(5000);
        server.Connected += (peer) =>
        {
            peer.MessageReceived += (message) =>
            {
                Debug.Log(timeSinceLast);
                timeSinceLast = 0;
                var packet = new TestClass();
                message.Deserialize(packet);
                var rotation = packet.quat;
                using (StateEvent.From(att, out InputEventPtr ptr))
                {
                    att.attitude.WriteValueIntoEvent(rotation, ptr);
                    InputSystem.QueueEvent(ptr);
                }
            };
        };
        client.Connect("127.0.0.1", 5000);
    }
    private void OnEnable()
    {
        InputSystem.EnableDevice(att);
    }
    private void OnDisable()
    {
        InputSystem.DisableDevice(att);
    }
    
    private void OnDestroy()
    {
        InputSystem.RemoveDevice(att);
    }

    private void Update()
    {

    }
}
