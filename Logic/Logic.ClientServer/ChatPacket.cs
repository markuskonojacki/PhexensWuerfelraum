using System;

namespace PhexensWuerfelraum.Logic.ClientServer
{
    public enum MessageType
    {
        Text,
        Action,
        Roll,
        Whisper
    }

    [Serializable]
    public class ChatPacket
    {
        public string Username { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public string UserColor { get; set; }
        public Guid RecipientGuid { get; set; } = Guid.Empty;
        public Guid SenderGuid { get; set; } = Guid.Empty;
        public MessageType MessageType { get; set; } = MessageType.Text;
    }
}