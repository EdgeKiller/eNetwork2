using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace eNetwork2
{
    /// <summary>
    /// eClient
    /// </summary>
    public class eClient
    {

        #region Variables

        private string Hostname;
        private int Port, PortRequest;

        private List<Task> TaskList;

        private TcpClient Client, ClientRequest;

        public delegate void DataReceivedHandler(byte[] buffer);
        public event DataReceivedHandler OnDataReceived;

        public delegate void ConnectionHandler();
        public event ConnectionHandler OnConnected, OnDisconnected;

        private int ID = -1;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor with hostname and the two ports
        /// </summary>
        /// <param name="hostname">Hostname</param>
        /// <param name="port">Port</param>
        /// <param name="portRequest">Port for request</param>
        public eClient(string hostname, int port, int portRequest)
        {
            Hostname = hostname;
            Port = port;
            PortRequest = portRequest;
            Client = new TcpClient();
            ClientRequest = new TcpClient();
            TaskList = new List<Task>();
        }

        #endregion

        #region PublicMethodsAndFunctions

        /// <summary>
        /// Connect the client
        /// </summary>
        public void Connect()
        {
            Client.Connect(Hostname, Port);
            ClientRequest.Connect(Hostname, PortRequest);

            byte[] idBuffer = new byte[4];
            Client.GetStream().Read(idBuffer, 0, idBuffer.Length);

            using (PacketReader pr = new PacketReader(idBuffer))
            {
                ID = pr.ReadInt32();
            }

            if (OnConnected != null)
                OnConnected();
            StartHandle();
        }

        /// <summary>
        /// Disconnect the client
        /// </summary>
        public void Disconnect()
        {
            if (Client.Connected)
                Client.Close();

            if (ClientRequest.Connected)
                ClientRequest.Close();

            if (OnDisconnected != null)
                OnDisconnected();

            Task.WhenAll(TaskList).Wait();
        }

        /// <summary>
        /// Send a buffer
        /// </summary>
        /// <param name="buffer">Buffer to send</param>
        public void Send(byte[] buffer)
        {
            byte[] b = Utils.GetBuffer(buffer);
            Client.Send(b);
        }

        /// <summary>
        /// Send a request
        /// </summary>
        /// <param name="buffer">Buffer to send</param>
        /// <returns>Response</returns>
        public byte[] SendRequest(byte[] buffer)
        {
            byte[] b = Utils.GetBuffer(buffer);
            ClientRequest.Send(b);

            byte[] result, resultSize = new byte[2];
            int size;

            try
            {
                ClientRequest.GetStream().Read(resultSize, 0, resultSize.Length);

                using (PacketReader pr = new PacketReader(resultSize))
                {
                    size = pr.ReadInt16();
                }

                result = new byte[size];
                ClientRequest.GetStream().Read(result, 0, result.Length);
                return result;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Get the ID
        /// </summary>
        /// <returns>ID</returns>
        public int GetID()
        {
            return ID;
        }

        #endregion

        #region PrivateMethodsAndFunctions

        /// <summary>
        /// Start handle client
        /// </summary>
        private void StartHandle()
        {
            TaskList.Add(HandleAsync());
        }

        /// <summary>
        /// Handle client
        /// </summary>
        /// <returns></returns>
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

                    using (PacketReader pr = new PacketReader(bufferSize))
                    {
                        size = pr.ReadInt16();
                    }

                    buffer = new byte[size];
                    await clientStream.ReadAsync(buffer, 0, buffer.Length);
                    if (OnDataReceived != null)
                        OnDataReceived(buffer);
                }
            }
            catch { }

            Disconnect();
        }

        #endregion
    }
}
