using System;
using System.Collections.Generic;

namespace PhexensWuerfelraum.Logic.ClientServer
{
    public enum UserType
    {
        Player,
        GameMaster
    }

    [Serializable]
    public class UserConnectionPacket
    {
        public string Username { get; set; }
        public UserType UserType { get; set; }
        public string UserGuid { get; set; }
        public List<UserModel> Users { get; set; }
        public bool IsJoining { get; set; }
    }
}