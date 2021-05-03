using System;

namespace PhexensWuerfelraum.Logic.ClientServer
{
    public enum UserType
    {
        Player,
        GameMaster,
        Admin
    }

    [Serializable]
    public class UserModel
    {
        public UserModel(string userName, UserType userType, Guid userIdentifier)
        {
            UserName = userName;
            UserType = userType;
            UserIdentifier = userIdentifier;
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public UserType UserType { get; set; }
        public bool IsGameMaster { get => UserType == UserType.GameMaster; }
        public bool IsPlayer { get => UserType == UserType.Player; }
        public Guid UserIdentifier { get; set; }
    }
}