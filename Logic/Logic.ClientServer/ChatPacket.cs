using SimpleSockets;
using SimpleSockets.Messaging.Metadata;
using System;
using System.Text;

namespace PhexensWuerfelraum.Logic.ClientServer
{
    public enum ChatMessageType
    {
        Text,
        Action,
        Roll,
        Whisper,
        RollWhisper
    }

    public class ChatPacket
    {
        public ChatPacket(ChatMessageType messageType, string message, int fromId, string fromUsername, int toId, string toUsername, string color = "Black")
        {
            MessageType = messageType;
            Message = message;
            FromId = fromId;
            FromUsername = fromUsername;
            ToId = toId;
            ToUsername = toUsername;
            UserColor = color;
            DateTime = DateTime.Now;
        }

        public ChatMessageType MessageType { get; set; } = ChatMessageType.Text;
        public DateTime DateTime { get; set; }
        public string UserColor { get; set; }

        public int FromId { get; set; }
        public string FromUsername { get; set; }

        public int ToId { get; set; }
        public string ToUsername { get; set; } = "";

        public string Message { get; set; }

        public object DeserializeToObject(byte[] objectBytes)
        {
            return Encoding.UTF8.GetString(objectBytes);
        }

        public event Action<SimpleSocket, IClientInfo, object, string> OnMessageReceived;

        public void RaiseOnMessageReceived(SimpleSocket socket, IClientInfo client, object message, string header)
        {
            OnMessageReceived?.Invoke(socket, client, message, header);
        }
    }
}