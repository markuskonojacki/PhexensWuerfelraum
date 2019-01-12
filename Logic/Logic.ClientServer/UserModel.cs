using System;

namespace PhexensWuerfelraum.Logic.ClientServer
{
    [Serializable]
    public class UserModel
    {
        public string UserGuid { get; set; }
        public string UserName { get; set; }
        public UserType UserType { get; set; }
        public bool IsGameMaster { get => UserType == UserType.GameMaster; }
        public bool IsPlayer { get => UserType == UserType.Player; }
    }
}