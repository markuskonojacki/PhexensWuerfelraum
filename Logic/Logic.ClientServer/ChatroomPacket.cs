using System;
using System.Collections.Generic;
using System.Text;

namespace PhexensWuerfelraum.Logic.ClientServer
{
    [Serializable]
    public class ChatroomPacket
    {
        public ChatroomPacket(List<UserModel> users)
        {
            Users = users;
        }

        public List<UserModel> Users { get; set; }
    }
}