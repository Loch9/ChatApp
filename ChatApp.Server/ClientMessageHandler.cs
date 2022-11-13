using System;
using System.Text;
using System.Collections.Generic;

using cslog;
using Riptide;

namespace ChatApp.Server
{
    public class ClientMessageHandler
    {
        public static string Room = "";

        [MessageHandler((ushort)ClientToServerId.message)]
        private static void RecieveMessage(ushort fromClientId, Message message)
        {
            string s = message.GetString();

            WriteMessage(s, fromClientId);
            SendMessage(fromClientId, s);
        }

        private static void SendMessage(ushort fromClientId, string msg)
        {
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.message);
            message.AddUShort(fromClientId);
            message.AddString(msg);
            NetworkManager.Server.SendToAll(message, fromClientId);
        }

        public static void SendMessages(ushort toClient, string messages)
        {
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.messages);

            string[] msgs = messages.Split("\n");
            if(msgs.Any(x => x == ""))
            {
                msgs = msgs.ToList().RemoveElement("").ToArray();
            }

            message.AddStrings(msgs);
            NetworkManager.Server.Send(message, toClient);
        }

        private static void WriteMessage(string message, ushort id)
        {
            if (!File.Exists("assets/rooms.ca"))
                throw new Exception("File 'assets/rooms.ca' not found. Please create a room before starting a server!");

            Directory.CreateDirectory("assets/rooms");

            if (!File.Exists("assets/rooms/" + Room + ".ca"))
                File.Create("assets/rooms/" + Room + ".ca").Close();
            FileStream fs = new FileStream("assets/rooms/" + Room + ".ca", FileMode.Append, FileAccess.Write);
            DateTime now = DateTime.Now;
            fs.Write(Encoding.UTF8.GetBytes(now.ToString("[dd/MM/yyyy | HH:mm:ss] ") + (User.list.TryGetValue(id, out User? user) ? user.Name + ": " : $"Unknown User {id}: ") + message + "\n"));
            fs.Close();
        }

        public static void CreateRoom(string name, ushort id)
        {
            Room = $"{id}-{name}";
            string roomName = $"{id}:{name}";
            if (!File.Exists("assets/rooms.ca"))
                File.Create("assets/rooms.ca").Close();
            FileStream fs = new FileStream("assets/rooms.ca", FileMode.Append, FileAccess.Write);
            fs.Write(Encoding.UTF8.GetBytes(roomName + "\n"));
            fs.Close();
        }
    }
}
