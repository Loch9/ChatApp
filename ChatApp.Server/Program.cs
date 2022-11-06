using System;
using Riptide;

namespace ChatApp.Server
{
    internal class Program
    {
        private static bool running = true;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += delegate(object ? sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                running = false;
            };

            NetworkManager.MaxClientCount = 10;
            NetworkManager.Port = 3466;

            NetworkManager.Start();

            while (running) NetworkManager.Update();
            NetworkManager.ServerClose();
        }
    }
}