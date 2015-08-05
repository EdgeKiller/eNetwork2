using System;
using System.Net.Sockets;
using eNetwork2;
using System.Data.SQLite;
using System.IO;

namespace LoginServer
{
    class Program
    {
        static eServer server;
        static DatabaseManager dbmanager;

        static void Main(string[] args)
        {
            server = new eServer(666, 667);
            server.OnClientConnected += Server_OnClientConnected;
            server.OnClientDisconnected += Server_OnClientDisconnected;
            server.OnRequestReceived += Server_OnRequestReceived;

            dbmanager = new DatabaseManager("db/database.db");

            server.Start();

            Log("Server started, waiting for clients ...");

            string command;

            while (true)
            {
                command = Console.ReadLine().Trim();
                if (command.Equals("/exit"))
                    break;

            }

            server.Stop();
        }

        private static void Server_OnRequestReceived(TcpClient client, byte[] buffer)
        {
            PacketReader reader = new PacketReader(buffer);
            byte ID = reader.ReadByte();


            if (ID == 1) //Login request
            {
                Log("Connection request received :");
                string username = reader.ReadString();
                string password = reader.ReadString();
                byte[] result = dbmanager.LoginRequest(username, password);
                server.SendTo(result, client);
            }
        }

        private static void Server_OnClientDisconnected(eSClient client)
        {
            Log("Client disconnected with ID : " + client.GetID());
        }

        private static void Server_OnClientConnected(eSClient client)
        {
            Log("New client connected with ID : " + client.GetID());
        }

        public static void Log(object message)
        {
            Console.WriteLine(message);
        }
    }
}
