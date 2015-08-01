using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace eNetwork2
{
    public class eClient
    {

        //Fields

        private string Hostname;
        private int Port;

        private List<Task> TaskList;

        private TcpClient Client;

        public delegate void DataReceivedHandler(byte[] buffer);
        public event DataReceivedHandler OnDataReceived;

        public delegate void ConnectionHandler();
        public event ConnectionHandler OnConnected, OnDisconnected;

        private int ID = -1;

        //Constructors

        public eClient(string hostname, int port)
        {
            Hostname = hostname;
            Port = port;
            Client = new TcpClient();
            TaskList = new List<Task>();
        }

        //Methods and functions

        public void Connect()
        {
            Client.Connect(Hostname, Port);

            byte[] idBuffer = new byte[4];
            Client.GetStream().Read(idBuffer, 0, idBuffer.Length);

            PacketReader pr = new PacketReader(idBuffer);
            ID = pr.ReadInt32();

            if (OnConnected != null)
                OnConnected();
            StartHandle();
        }

        public void Disconnect()
        {
            if(Client.Connected)
                Client.Close();
            if (OnDisconnected != null)
                OnDisconnected();
            Task.WhenAll(TaskList).Wait();
        }

        public void Send(byte[] buffer)
        {
            byte[] b = Utils.GetBuffer(buffer);
            Client.GetStream().Write(b, 0, b.Length);
        }

        public int GetID()
        {
            return ID;
        }

        private void StartHandle()
        {
            TaskList.Add(HandleAsync());
        }

        private async Task HandleAsync()
        {
            NetworkStream clientStream = Client.GetStream();
            byte[] buffer, bufferSize = new byte[2];
            int size;

            try
            {
                while (Client.Connected)
                {
                    await clientStream.ReadAsync(bufferSize, 0, bufferSize.Length);

                    PacketReader pr = new PacketReader(bufferSize);
                    size = pr.ReadInt16();

                    buffer = new byte[size];
                    await clientStream.ReadAsync(buffer, 0, buffer.Length);
                    if (OnDataReceived != null)
                        OnDataReceived(buffer);
                }
            }
            catch { }

            Disconnect();
        }
    }
}
