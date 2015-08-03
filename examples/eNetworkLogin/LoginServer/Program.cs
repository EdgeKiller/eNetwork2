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

        static void Main(string[] args)
        {
            CheckForDatabases();

            server = new eServer(666, 667);
            server.OnClientConnected += Server_OnClientConnected;
            server.OnClientDisconnected += Server_OnClientDisconnected;
            server.OnRequestReceived += Server_OnRequestReceived;

            server.Start();

            string command;

            while (true)
            {
                command = Console.ReadLine().Trim();
                if (command.Equals("/exit"))
                    break;

            }

            server.Stop();
        }

        private static void CheckForDatabases()
        {
            if (!Directory.Exists("db"))
                Directory.CreateDirectory("db");

            if (!File.Exists("db/database.db"))
                SQLiteConnection.CreateFile("db/database.db");

            /*
            SQLiteConnection connection = new SQLiteConnection("Data Source=db/database.db;Version=3;");
            connection.Open();


            SQLiteCommand commande = new SQLiteCommand("insert into accounts (username, password) values ('ttt', 'ttt')", connection);
            commande.ExecuteNonQuery();



            connection.Close();*/
        }

        private static void Server_OnRequestReceived(TcpClient client, byte[] buffer)
        {
            PacketReader reader = new PacketReader(buffer);
            byte ID = reader.ReadByte();


            if (ID == 1) //Login request
            {
                Log("Connection request received");
                string username = reader.ReadString();
                string password = reader.ReadString();
                SQLiteConnection connection = new SQLiteConnection("Data Source=db/database.db;Version=3;");
                connection.Open();

                byte[] response = null;

                SQLiteCommand command = new SQLiteCommand("select * from accounts where username='" + username + "'", connection);
                SQLiteDataReader sqlreader = command.ExecuteReader();
                if (sqlreader.Read())
                {
                    if (password == sqlreader["password"].ToString())
                    {
                        Log("Connection request accepted");
                        response = new byte[1];
                        using (PacketWriter pw = new PacketWriter())
                        {
                            pw.WriteBoolean(true);
                            response = pw.ToArray();
                        }
                    }
                }

                if (response == null)
                {
                    Log("Connection request failed");
                    using (PacketWriter pw = new PacketWriter())
                    {
                        pw.WriteBoolean(false);
                        response = pw.ToArray();
                    }
                }

                server.SendTo(response, client);

                connection.Close();
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

        private static void Log(object message)
        {
            Console.WriteLine(message);
        }
    }
}
