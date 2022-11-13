using System;
using System.Linq;
using System.IO;
using System.Text;
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

            Directory.CreateDirectory("assets");

            Console.Write("Enter room name: ");
            string? name = Console.ReadLine();

            if (string.IsNullOrEmpty(name))
                return;

            if (File.Exists("assets/IDs.ca"))
            {
                FileStream fs = new FileStream("assets/IDs.ca", FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(fs);
                var ids = reader.ReadToEnd().Split("\n").ToList();

                ids = ids[0] == "" ? new List<string>() : ids;

                if(ids.Any(x => x == ""))
                {
                    if (ids.Contains("")) _ = ids.RemoveElement("");
                }

                ushort uid;

                List<string> names = new List<string>();

                FileStream fsNames = new FileStream("assets/rooms.ca", FileMode.Open, FileAccess.Read);
                StreamReader namesReader = new StreamReader(fsNames);
                names = namesReader.ReadToEnd().Split("\n").ToList().RemoveElement("");

                namesReader.Close();

                List<string> temp = new List<string>();
                foreach(string s in names)
                {
                    temp.Add(s.Split(":")[1]);
                }

                names = temp;

                bool addToFile = false;

                if (ids.Count == 0)
                {
                    uid = 1;
                    addToFile = true;
                }
                else if (names.Contains(name))
                {
                    uid = (ushort)(names.FindIndex(x => x == name) + 1);
                    addToFile = false;
                }
                else
                {
                    var uids = ids.ConvertAll(x => Convert.ToUInt16(x));
                    uid = (ushort)(uids.Max() + 1);
                    addToFile = true;
                }

                ClientMessageHandler.CreateRoom(name, uid);

                fs.Close();

                if (addToFile)
                {
                    fs = new FileStream("assets/IDs.ca", FileMode.Append, FileAccess.Write);
                    fs.WriteAsync(Encoding.UTF8.GetBytes($"{uid}\n"));
                    fs.Close();
                }
            }
            else
            {
                File.Create("assets/IDs.ca").Close();
                FileStream fs = new FileStream("assets/IDs.ca", FileMode.Append, FileAccess.Write);

                ClientMessageHandler.CreateRoom(name, 1);
                fs.WriteAsync(Encoding.UTF8.GetBytes($"{(ushort)1}\n"));
                fs.Close();
            }

            NetworkManager.MaxClientCount = 10;
            NetworkManager.Port = 3466;

            NetworkManager.Start();

            while (running) NetworkManager.Update();
            NetworkManager.ServerClose();
        }
    }
}