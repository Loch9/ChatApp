using System;
using System.Collections.Generic;

using cslog;
using Riptide;
using Riptide.Utils;

namespace ChatApp.Server
{
    public enum ClientToServerId : ushort
    {
        name = 1,
        message,
    }

    public enum ServerToClientId : ushort
    {
        message = 1,
        names,
        messages
    }

    public static class NetworkManager
    {
        public static ushort Port;
        public static ushort MaxClientCount;

        public static Logger Logger;

        public static Riptide.Server Server;

        public static void Start()
        {
            Logger = Logger.Create("(SERVER)", "!@[!t] !v!*");
            Logger.SetLogMethod(Console.WriteLine);
            
            RiptideLogger.Initialize(Logger.LogDebug, Logger.LogInfo, Logger.LogWarning, Logger.LogError, false);

            Server = new Riptide.Server(new Riptide.Transports.Udp.UdpServer(Riptide.Transports.Udp.SocketMode.IPv4Only));
            Server.Start(Port, MaxClientCount);
            Server.ClientConnected += ClientConnected;
            Server.ClientDisconnected += ClientDisconnected;
        }

        public static void Update()
        {
            Server.Update();
        }

        public static void ClientConnected(object? sender, ServerConnectedEventArgs e)
        {
            User.list.Add(e.Client.Id, new User());

            if (!(string.IsNullOrEmpty(ClientMessageHandler.Room)) && File.Exists("assets/rooms/" + ClientMessageHandler.Room + ".ca")) 
            {
                FileStream fs = new FileStream("assets/rooms/" + ClientMessageHandler.Room + ".ca", FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(fs);
                ClientMessageHandler.SendMessages(e.Client.Id, reader.ReadToEnd());

                reader.Close();
            }
        }

        public static void ClientDisconnected(object? sender, ServerDisconnectedEventArgs e)
        {

        }

        public static void ServerClose()
        {
            Server.Stop();
        }
    }
}
