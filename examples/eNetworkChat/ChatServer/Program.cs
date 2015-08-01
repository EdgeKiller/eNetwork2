using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eNetwork2;

namespace ChatServer
{
    class Program
    {
        static eServer server;

        static void Main(string[] args)
        {
            server = new eServer(666);
            server.OnDataReceived += server_OnDataReceived;

            server.Start();

            string command;

            while(true)
            {
                command = Console.ReadLine().Trim();
                if(command.Equals("/exit"))
                {
                    server.Stop();
                    Environment.Exit(0);
                }
            }
        }

        static void server_OnDataReceived(eSClient client, byte[] buffer)
        {
            PacketReader pr = new PacketReader(buffer);
            Byte ID = pr.ReadByte();

            if(ID == 1)
            {
                server.SendToAll(buffer);
                string message = pr.ReadString();
                Console.WriteLine("Message received : " + message);
            }

        }
    }
}
