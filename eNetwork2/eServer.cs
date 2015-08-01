using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace eNetwork2
{
    public class eServer
    {
        //Fields

        private int Port;

        private TcpListener Listener;

        private List<Task> TaskList;

        private List<eSClient> ClientList;

        public delegate void DataReceivedHandler(eSClient client, byte[] buffer);
        public event DataReceivedHandler OnDataReceived;

        public delegate void ConnectionHandler(eSClient client);
        public event ConnectionHandler OnClientConnected, OnClientDisconnected;

        private Random randomID;

        //Constructors

        public eServer(int port)
        {
            Port = port;
            Listener = new TcpListener(IPAddress.Any, this.Port);
            TaskList = new List<Task>();
            ClientList = new List<eSClient>();
            randomID = new Random();
        }

        //Methods and functions

        public void Start()
        {
            Listener.Start();
            StartListen();
        }

        public void Stop()
        {
            Listener.Stop();
            //Task.WhenAll(TaskList).Wait();
        }

        public void SendToAll(byte[] buffer)
        {
            foreach (eSClient client in ClientList)
            {
                byte[] b = Utils.GetBuffer(buffer);
                client.GetTcpClient().GetStream().Write(b, 0, b.Length);
            }
        }

        public void SendToAllExcept(byte[] buffer, eSClient exceptedClient)
        {
            foreach (eSClient client in ClientList)
            {
                if (client != exceptedClient)
                {
                    byte[] b = Utils.GetBuffer(buffer);
                    client.GetTcpClient().GetStream().Write(b, 0, b.Length);
                }
            }
        }

        public List<eSClient> GetClientList()
        {
            return ClientList;
        }

        private void StartListen()
        {
            TaskList.Add(ListenAsync());
        }

        private async Task ListenAsync()
        {
            while (true)
            {
                TcpClient client = await this.Listener.AcceptTcpClientAsync();
                Int32 id = (Int32)randomID.Next(10000, 99999);
                eSClient sClient = new eSClient(id, client);

                byte[] idBuffer = new byte[4];

                PacketWriter.WriteInt32(ref idBuffer, id);

                client.GetStream().Write(idBuffer, 0, idBuffer.Length);

                ClientList.Add(sClient);
                if (OnClientConnected != null)
                    OnClientConnected(sClient);
                StartHandleClient(sClient);
            }
        }

        private void StartHandleClient(eSClient client)
        {
            TaskList.Add(HandleClientAsync(client));
        }

        private async Task HandleClientAsync(eSClient client)
        {
            NetworkStream clientStream = client.GetTcpClient().GetStream();
            byte[] buffer, bufferSize = new byte[2];
            int size;

            try
            {
                while (client.GetTcpClient().Client.Connected)
                {
                    await clientStream.ReadAsync(bufferSize, 0, bufferSize.Length);

                    PacketReader pr = new PacketReader(bufferSize);
                    size = pr.ReadInt16();

                    buffer = new byte[size];
                    await clientStream.ReadAsync(buffer, 0, buffer.Length);
                    if (OnDataReceived != null)
                        OnDataReceived(client, buffer);
                }
            }
            catch { }

            ClientList.Remove(client);
            if (OnClientDisconnected != null)
                OnClientDisconnected(client);
        }

    }
}
