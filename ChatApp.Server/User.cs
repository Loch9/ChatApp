using System;
using System.Collections.Generic;

using Riptide;

namespace ChatApp.Server
{
    public class User
    {
        public static Dictionary<ushort, User> list = new Dictionary<ushort, User>();
        public static Dictionary<ushort, string> names = new Dictionary<ushort, string>();

        public int Id { get; set; }
        public string Name { get; set; }

        public User()
        {
            Name = "";
        }

        [MessageHandler((ushort)ClientToServerId.name)]
        private static void GetName(ushort fromClientId, Message message)
        {
            if (list.TryGetValue(fromClientId, out User? user)) 
            {
                user.Name = message.GetString();
                names.Add(fromClientId, user.Name);

                NetworkManager.Logger.LogInfo($"Client connected with name: {user.Name}", "!@[!t] !n: !v!*");
            }

            SendNames();
        }

        private static void SendNames()
        {
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.names);

            message.AddInt(names.Count);

            for (int i = 0; i < names.Count; i++)
            {
                message.AddUShort(names.ElementAt(i).Key);
                message.AddString(names.ElementAt(i).Value);
            }
            NetworkManager.Server.SendToAll(message);
        }
    }
}
