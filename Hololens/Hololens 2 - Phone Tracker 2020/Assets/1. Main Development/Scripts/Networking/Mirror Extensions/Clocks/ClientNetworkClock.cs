using System;
using Mirror;
using System.Runtime.CompilerServices;

[Serializable]
public class ClientNetworkClock : BaseNetworkClock
{
    public int windowSize = 10;
    private ExponentialMovingAverage rtt, offset;
    private double offsetMin = double.MinValue, offsetMax = double.MaxValue;

    public double Latency => rtt.Value * 0.5f;
    public double LastPingTime { get; private set; }
    public ClientNetworkClock() {
        Reset();
    }
    public ClientNetworkClock(int windowSize)
    {
        this.windowSize = windowSize;
        Reset();
    }
    public override double Time
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => LocalTime - offset.Value;
    }
    public override void Reset()
    {
        LastPingTime = 0;
        rtt = new ExponentialMovingAverage(windowSize);
        offset = new ExponentialMovingAverage(windowSize);
        offsetMin = double.MinValue;
        offsetMax = double.MaxValue;
    }

    public PingMessage GetPingMessage()
    {
        var local = LocalTime;
        var m = new PingMessage(local);
        LastPingTime = local;
        return m;
    }

    public void Update(PongMessage pong)
    {
        double now = LocalTime;
        double serverTime = pong.serverTime;
        double clientTime = pong.clientTime;

        // how long did this message take to come back
        double newRtt = now - clientTime;
        rtt.Add(newRtt);
        
        // the difference in time between the client and the server
        // but subtract half of the rtt to compensate for latency
        // half of rtt is the best approximation we have
        double newOffset = now - newRtt * 0.5f - serverTime;

        double newOffsetMin = now - newRtt - serverTime;
        double newOffsetMax = now - serverTime;
        offsetMin = Math.Max(offsetMin, newOffsetMin);
        offsetMax = Math.Min(offsetMax, newOffsetMax);

        if (offset.Value < offsetMin || offset.Value > offsetMax)
        {
            // the old offset was offrange,  throw it away and use new one
            offset = new ExponentialMovingAverage(windowSize);
            offset.Add(newOffset);
        }
        else if (newOffset >= offsetMin || newOffset <= offsetMax)
        {
            // new offset looks reasonable,  add to the average
            offset.Add(newOffset);
        }
    } 
}
