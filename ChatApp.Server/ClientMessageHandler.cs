using System;
using System.Collections.Generic;

using cslog;
using Riptide;

namespace ChatApp.Server
{
    public class ClientMessageHandler
    {
        [MessageHandler((ushort)ClientToServerId.message)]
        private static void RecieveMessage(ushort fromClientId, Message message)
        {
            string s = message.GetString();

            SendMessage(fromClientId, s);
        }

        private static void SendMessage(ushort fromClientId, string msg)
        {
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.message);
            message.AddUShort(fromClientId);
            message.AddString(msg);
            NetworkManager.Server.SendToAll(message, fromClientId);
        }
    }
}
