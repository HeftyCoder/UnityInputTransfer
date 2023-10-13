using System;
using System.Runtime.CompilerServices;
public class ServerNetworkClock : BaseNetworkClock
{
    public override double Time => LocalTime;

    public PongMessage GetPongMessage(PingMessage ping)
    {
        var pong = new PongMessage(ping.clientTime, LocalTime);
        return pong;
    }
}