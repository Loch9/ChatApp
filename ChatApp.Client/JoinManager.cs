using System;
using System.Collections.Generic;

using cslog;
using Riptide;

namespace ChatApp.Client
{
    public static class JoinManager
    {
        public static string NameField = "";
        public static string IPField = "";

        public static void Start()
        {
            Console.Write("Room's IP adress: ");
            string? ip = Console.ReadLine();
            if (string.IsNullOrEmpty(ip))
            {
                ip = "127.0.0.1";
            }

            IPField = ip.Trim();

            Console.Write("Connect to chat room with username: ");
            string? name = Console.ReadLine();
            if (string.IsNullOrEmpty(name))
            {
                name = "";
            }

            NameField = name;
        }

        public static void SendName()
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.name);
            message.AddString(NameField);
            NetworkManager.Client.Send(message);
        }

        public static void Update()
        {
            if (Program.isRunning)
            {
                string? message = Console.ReadLine();

                if (string.IsNullOrEmpty(message))
                    Update();
                else
                {
                    if (User.list.TryGetValue(NetworkManager.Client.Id, out User? user))
                    {
                        user.SendMessage(message);
                        if (NetworkManager.Loggers.TryGetValue(User.list.FirstOrDefault(x => x.Value == user).Key, out Logger? logger))
                        {
                            Console.CursorTop--;
                            logger.Log(LogLevel.None, message);
                        }
                    }
                    Update();
                }
            }
        }

        [MessageHandler((ushort)ServerToClientId.message)]
        private static void RecieveMessage(Message message)
        {
            ushort id = message.GetUShort();
            string msg = message.GetString();

            if(NetworkManager.Loggers.TryGetValue(id, out Logger? logger))
            {
                logger.Log(LogLevel.None, msg);
            }
        }
    }
}
