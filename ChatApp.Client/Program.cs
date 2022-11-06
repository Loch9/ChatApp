using System;
using cslog;

namespace ChatApp.Client
{
    internal class Program
    {
        public static bool isRunning = true;

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                isRunning = false;
            };

            JoinManager.Start();

            NetworkManager.Port = 3466;
            NetworkManager.IP = JoinManager.IPField;

            NetworkManager.Start();

            NetworkManager.Connect();

            while (isRunning) NetworkManager.Update();
            NetworkManager.Disconnect();
        }
    }
}