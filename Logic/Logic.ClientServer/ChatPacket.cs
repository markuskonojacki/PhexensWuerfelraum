using System;

namespace PhexensWuerfelraum.Logic.ClientServer
{
    public enum MessageType
    {
        Text,
        Action,
        Roll,
        Whisper,
        RollWhisper
    }

    [Serializable]
    public class ChatPacket
    {
        public MessageType MessageType { get; set; } = MessageType.Text;
        public DateTime DateTime { get; set; }
        public string UserColor { get; set; }

        public Guid SenderGuid { get; set; } = Guid.Empty;
        public string FromUsername { get; set; }

        public Guid RecipientGuid { get; set; } = Guid.Empty;
        public string ToUsername { get; set; } = "";

        public string Message { get; set; }
    }
}