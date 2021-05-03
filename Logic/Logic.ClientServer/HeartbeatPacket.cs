using System;

namespace PhexensWuerfelraum.Logic.ClientServer
{
    [Serializable]
    public class HeartbeatPacket
    {
        public HeartbeatPacket(Guid guid, DateTime firstBeat, DateTime lastBeat)
        {
            Guid = guid;
            FirstBeat = firstBeat;
            LastBeat = lastBeat;
        }

        public Guid Guid { get; }
        public DateTime FirstBeat { get; }
        public DateTime LastBeat { get; }
    }
}