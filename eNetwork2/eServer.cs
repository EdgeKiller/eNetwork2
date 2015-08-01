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

        private List<TcpClient> ClientList;

        public delegate void DataReceivedHandler(TcpClient client, byte[] buffer);
        public event DataReceivedHandler OnDataReceived;

        public delegate void ConnectionHandler(TcpClient client);
        public event ConnectionHandler OnClientConnected, OnClientDisconnected;

        //Constructors

        public eServer(int port)
        {
            Port = port;
            Listener = new TcpListener(IPAddress.Any, this.Port);
            TaskList = new List<Task>();
            ClientList = new List<TcpClient>();
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
            Task.WhenAll(TaskList).Wait();
        }

        public void SendToAll(byte[] buffer)
        {
            foreach (TcpClient client in ClientList)
            {
                byte[] b = Utils.GetBuffer(buffer);
                client.GetStream().Write(b, 0, b.Length);
            }
        }

        public void SendToAllExcept(byte[] buffer, TcpClient exceptedClient)
        {
            foreach(TcpClient client in ClientList)
            {
                if (client != exceptedClient)
                {
                    byte[] b = Utils.GetBuffer(buffer);
                    client.GetStream().Write(b, 0, b.Length);
                }
            }
        }

        private void StartListen()
        {
            TaskList.Add(ListenAsync());
        }

        private async Task ListenAsync()
        {
            while(true)
            {
                TcpClient client = await this.Listener.AcceptTcpClientAsync();
                ClientList.Add(client);
                if (OnClientConnected != null)
                    OnClientConnected(client);
                StartHandleClient(client);
            }
        }

        private void StartHandleClient(TcpClient client)
        {
            TaskList.Add(HandleClientAsync(client));
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            NetworkStream clientStream = client.GetStream();
            byte[] buffer, bufferSize = new byte[2];
            int size;

            try
            {
                while (client.Client.Connected)
                {
                    await clientStream.ReadAsync(bufferSize, 0, bufferSize.Length);
                    using (MemoryStream ms = new MemoryStream(bufferSize))
                    {
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            size = br.ReadInt16();
                        }
                    }
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
