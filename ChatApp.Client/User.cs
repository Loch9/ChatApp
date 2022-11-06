using System;
using System.Collections.Generic;

using Riptide;

namespace ChatApp.Client
{
    public class User
    {
        public static Dictionary<ushort, User> list = new Dictionary<ushort, User>();

        public int Id { get; set; }
        public string Name { get; set; }

        public User(string name)
        {
            Name = name;
        }

        public void SendMessage(string msg)
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.message);
            message.AddString(msg);
            NetworkManager.Client.Send(message);
        }
    }
}
