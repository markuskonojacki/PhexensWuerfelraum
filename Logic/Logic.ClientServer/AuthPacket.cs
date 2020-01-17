using System;

namespace PhexensWuerfelraum.Logic.ClientServer
{
    [Serializable]
    public class AuthPacket
    {
        public AuthPacket(UserModel userModel, string password, string channel)
        {
            UserModel = userModel;
            Password = password;
            Channel = channel;
        }

        public UserModel UserModel { get; set; }
        public string Password { get; set; }
        public string Channel { get; set; }
    }
}