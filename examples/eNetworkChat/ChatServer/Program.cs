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
            server.OnClientConnected += Server_OnClientConnected;
            server.OnClientDisconnected += Server_OnClientDisconnected;
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

        private static void Server_OnClientDisconnected(eSClient client)
        {
            Console.WriteLine("Client disconnected : " + client.GetID());
        }

        private static void Server_OnClientConnected(eSClient client)
        {
            Console.WriteLine("Client connected : " + client.GetID());
        }

        static void server_OnDataReceived(eSClient client, byte[] buffer)
        {
            PacketReader pr = new PacketReader(buffer);
            Byte ID = pr.ReadByte();

            if(ID == 1)
            {
                string message = pr.ReadString();
                PacketWriter pw = new PacketWriter();
                pw.WriteByte(ID);
                pw.WriteInt32(server.GetIDFromTcpClient(client.GetTcpClient()));
                pw.WriteString(message);
                server.SendToAll(pw.ToArray());
                
                Console.WriteLine("Message received from " + client.GetID() + " : " + message);
            }

        }
    }
}
