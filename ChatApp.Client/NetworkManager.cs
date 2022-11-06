using System;
using System.Collections.Generic;

using cslog;
using Riptide;
using Riptide.Utils;

namespace ChatApp.Client
{
    public enum ClientToServerId : ushort
    {
        name = 1,
        message,
    }

    public enum ServerToClientId : ushort
    {
        message = 1,
        names
    }

    public static class NetworkManager
    {
        public static Riptide.Client Client = new Riptide.Client();

        public static string IP = "";
        public static ushort Port;

        public static LogPattern ClientPattern = "[!d | !t] !n: !v";

        public static Logger Logger = new Logger();

        public static Dictionary<ushort, Logger> Loggers = new Dictionary<ushort, Logger>();
        public static Dictionary<ushort, string> Names = new Dictionary<ushort, string>();

        public static void Start()
        {
            Logger = Logger.Create("(CLIENT)", "!@[!t] !v!*");
            Logger.SetLogMethod(Console.WriteLine);

            RiptideLogger.Initialize(Logger.LogDebug, Logger.LogInfo, Logger.LogWarning, Logger.LogFatal, false);

            Client = new Riptide.Client();
            Client.Connected += DidConnect;
            Client.ConnectionFailed += FailedToConnect;
            Client.ClientConnected += ClientConnected;
            Client.ClientDisconnected += ClientDisconnected;
            Client.Disconnected += DidDisconnect;
        }

        public static void Connect()
        {
            Client.Connect($"{IP}:{Port}");
        }

        public static void Disconnect()
        {
            Client.Disconnect();
        }

        public static void Update()
        {
            Client.Update();
        }

        private static void DidConnect(object? sender, EventArgs e)
        {
            JoinManager.SendName();
            User.list.Add(Client.Id, new User(JoinManager.NameField));

            CreateLoggers();

            Thread thread = new Thread(JoinManager.Update);
            thread.Start();
        }

        private static void FailedToConnect(object? sender, ConnectionFailedEventArgs e)
        {
            // Back to join chat room
            Console.Clear();
            Logger.LogFatal($"Failed to connect to the server: {(e.Message == null ? "No connection." : e.Message.ToString())}");
            JoinManager.Start();
        }

        private static void ClientConnected(object? sender, ClientConnectedEventArgs e)
        {
            User.list.Add(e.Id, new User(Names.TryGetValue(e.Id, out string? str) ? str : $"Unknown Guest {e.Id}"));

            Loggers = new Dictionary<ushort, Logger>();

            CreateLoggers();
        }

        private static void ClientDisconnected(object? sender, ClientDisconnectedEventArgs e)
        {
            // Remove client that left from connected clients list
            Names.Remove(e.Id);
            User.list.Remove(e.Id);
            Loggers.Remove(e.Id);
        }

        private static void DidDisconnect(object? sender, EventArgs e)
        {
            // Reset list of connected clients
            Names = new Dictionary<ushort, string>();
            User.list = new Dictionary<ushort, User>();
            Loggers = new Dictionary<ushort, Logger>();
        }

        [MessageHandler((ushort)ServerToClientId.names)]
        private static void GetNames(Message message)
        {
            int n = message.GetInt();

            Names = new Dictionary<ushort, string>();

            for (int i = 0; i < n; i++)
            {
                Names.Add(message.GetUShort(), message.GetString());
            }

            CreateLoggers();
        }

        private static void CreateLoggers()
        {
            for (int i = 0; i < Names.Count; i++)
            {
                if (!Loggers.Keys.Contains(Names.ElementAt(i).Key))
                    Loggers.Add(Names.ElementAt(i).Key, Logger.Create(Names.ElementAt(i).Value, ClientPattern));
            }

            foreach (Logger logger in Loggers.Values)
            {
                logger.SetLogMethod(Console.WriteLine);
            }
        }
    }
}
