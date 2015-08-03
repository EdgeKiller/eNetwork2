using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace eNetwork2
{
    /// <summary>
    /// eServer
    /// </summary>
    public class eServer
    {
        #region Variables

        private int Port, PortRequest;

        private TcpListener Listener, ListenerRequest;

        private List<Task> TaskList;

        private List<eSClient> ClientList;

        public delegate void DataReceivedHandler(eSClient client, byte[] buffer);
        public event DataReceivedHandler OnDataReceived;

        public delegate void RequestReceivedHandler(TcpClient client, byte[] buffer);
        public event RequestReceivedHandler OnRequestReceived;

        public delegate void ConnectionHandler(eSClient client);
        public event ConnectionHandler OnClientConnected, OnClientDisconnected;

        private Random randomID;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="port">Port</param>
        /// <param name="portRequest">Port for request</param>
        public eServer(int port, int portRequest)
        {
            Port = port;
            PortRequest = portRequest;

            Listener = new TcpListener(IPAddress.Any, Port);
            ListenerRequest = new TcpListener(IPAddress.Any, PortRequest);

            TaskList = new List<Task>();
            ClientList = new List<eSClient>();

            randomID = new Random();
        }

        #endregion

        #region PublicMethodsAndFunctions

        /// <summary>
        /// Start the server
        /// </summary>
        public void Start()
        {
            Listener.Start();
            ListenerRequest.Start();

            StartListen();
            StartListenRequest();
        }

        /// <summary>
        /// Stop the server
        /// </summary>
        public void Stop()
        {
            Listener.Stop();
            //Task.WhenAll(TaskList).Wait();
        }

        /// <summary>
        /// Send buffer to one client
        /// </summary>
        /// <param name="buffer">Buffer to send</param>
        /// <param name="client">Client</param>
        public void SendTo(byte[] buffer, eSClient client)
        {
            byte[] b = Utils.GetBuffer(buffer);
            client.GetTcpClient().GetStream().Write(b, 0, b.Length);
        }

        /// <summary>
        /// Send buffer to one client (Request)
        /// </summary>
        /// <param name="buffer">Buffer to send</param>
        /// <param name="client">Client</param>
        public void SendTo(byte[] buffer, TcpClient client)
        {
            byte[] b = Utils.GetBuffer(buffer);
            client.GetStream().Write(b, 0, b.Length);
        }

        /// <summary>
        /// Send buffer to all client connected
        /// </summary>
        /// <param name="buffer">Buffer to send</param>
        public void SendToAll(byte[] buffer)
        {
            foreach (eSClient client in ClientList)
            {
                byte[] b = Utils.GetBuffer(buffer);
                client.GetTcpClient().GetStream().Write(b, 0, b.Length);
            }
        }

        /// <summary>
        /// Send buffer to all client connected except one
        /// </summary>
        /// <param name="buffer">Buffer to send</param>
        /// <param name="exceptedClient">Excepted client</param>
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

        /// <summary>
        /// Get ID from a client
        /// </summary>
        /// <param name="client">Client to get the ID</param>
        /// <returns>ID</returns>
        public int GetIDFromTcpClient(TcpClient client)
        {
            foreach (eSClient c in ClientList)
            {
                if (c.GetTcpClient() == client)
                    return c.GetID();
            }
            return -1;
        }

        /// <summary>
        /// Get the list of connected clients
        /// </summary>
        /// <returns>List of client</returns>
        public List<eSClient> GetClientList()
        {
            return ClientList;
        }

        #endregion

        #region PrivateMethodsAndFunctions

        /// <summary>
        /// Start to listen for client
        /// </summary>
        private void StartListen()
        {
            TaskList.Add(ListenAsync());
        }

        /// <summary>
        /// Listen for client
        /// </summary>
        /// <returns></returns>
        private async Task ListenAsync()
        {
            while (true)
            {
                TcpClient client = await this.Listener.AcceptTcpClientAsync();
                Int32 id = (Int32)randomID.Next(10000, 99999);
                eSClient sClient = new eSClient(id, client);

                byte[] idBuffer;

                using (PacketWriter pw = new PacketWriter())
                {
                    pw.WriteInt32(id);
                    idBuffer = pw.ToArray();
                }

                client.GetStream().Write(idBuffer, 0, idBuffer.Length);

                ClientList.Add(sClient);
                if (OnClientConnected != null)
                    OnClientConnected(sClient);
                StartHandleClient(sClient);
            }
        }

        /// <summary>
        /// Start to handle client
        /// </summary>
        /// <param name="client">Client</param>
        private void StartHandleClient(eSClient client)
        {
            TaskList.Add(HandleClientAsync(client));
        }

        /// <summary>
        /// Handle client
        /// </summary>
        /// <param name="client">Client</param>
        /// <returns></returns>
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

                    using (PacketReader pr = new PacketReader(bufferSize))
                    {
                        size = pr.ReadInt16();
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

        /// <summary>
        /// Start to listen for request client
        /// </summary>
        private void StartListenRequest()
        {
            TaskList.Add(ListenRequestAsync());
        }

        /// <summary>
        /// Listen request client
        /// </summary>
        /// <returns></returns>
        private async Task ListenRequestAsync()
        {
            while (true)
            {
                TcpClient client = await this.ListenerRequest.AcceptTcpClientAsync();
                StartHandleClientRequest(client);
            }
        }

        /// <summary>
        /// Start handle for request client
        /// </summary>
        /// <param name="client">Client</param>
        private void StartHandleClientRequest(TcpClient client)
        {
            TaskList.Add(HandleClientRequestAsync(client));
        }

        /// <summary>
        /// Handle request client
        /// </summary>
        /// <param name="client">Client</param>
        /// <returns></returns>
        private async Task HandleClientRequestAsync(TcpClient client)
        {
            NetworkStream clientStream = client.GetStream();
            byte[] buffer, bufferSize = new byte[2];
            int size;

            try
            {
                while (client.Client.Connected)
                {
                    await clientStream.ReadAsync(bufferSize, 0, bufferSize.Length);

                    using (PacketReader pr = new PacketReader(bufferSize))
                    {
                        size = pr.ReadInt16();
                    }

                    buffer = new byte[size];
                    await clientStream.ReadAsync(buffer, 0, buffer.Length);

                    if (OnRequestReceived != null)
                        OnRequestReceived(client, buffer);
                }
            }
            catch { }


            client = null;
        }

        #endregion
    }
}
