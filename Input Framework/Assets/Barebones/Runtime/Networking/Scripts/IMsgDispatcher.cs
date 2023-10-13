using UnityEngine.Networking;

namespace Barebones.Networking
{
    public interface IMsgDispatcher
    {
        /// <summary>
        /// Peer, to which we have connected
        /// </summary>
        IPeer Peer { get; }

        void SendMessage(short opCode);
        void SendMessage<T>(short opCode, T packet) where T:ISerializablePacket;
        void SendMessage<T>(short opCode, T packet, DeliveryMethod method) where T:ISerializablePacket;
        void SendMessage<T>(short opCode, T packet, ResponseCallback responseCallback) where T : ISerializablePacket;
        void SendMessage<T>(short opCode, T packet, ResponseCallback responseCallback, int timeoutSecs) where T : ISerializablePacket;
        void SendMessage(short opCode, ResponseCallback responseCallback);
        void SendMessage(short opCode, byte[] data);
        void SendMessage(short opCode, byte[] data, ResponseCallback responseCallback);
        void SendMessage(short opCode, byte[] data, ResponseCallback responseCallback, int timeoutSecs);
        void SendMessage(short opCode, string data);
        void SendMessage(short opCode, string data, ResponseCallback responseCallback);
        void SendMessage(short opCode, string data, ResponseCallback responseCallback, int timeoutSecs);
        void SendMessage(short opCode, int data);
        void SendMessage(short opCode, int data, ResponseCallback responseCallback);
        void SendMessage(short opCode, int data, ResponseCallback responseCallback, int timeoutSecs);
        void SendMessage<T>(T message) where T : IMessage;
        void SendMessage<T>(T message, ResponseCallback responseCallback) where T : IMessage;
        void SendMessage<T>(T message, ResponseCallback responseCallback, int timeoutSecs) where T : IMessage;
    }
}