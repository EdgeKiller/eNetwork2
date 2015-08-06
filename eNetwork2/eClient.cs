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

        public bool Debug { get; set; } = false;

        public int RequestTimeout {
            get
            {
                if (ClientRequest != null)
                    return ClientRequest.ReceiveTimeout;
                else
                    return -1;
            }
            set
            {
                if (ClientRequest != null)
                    ClientRequest.ReceiveTimeout = value;
            }
        }

        private bool Connected;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor with hostname and the two ports
        /// </summary>
        /// <param name="hostname">Hostname</param>
        /// <param name="port">Port</param>
        /// <param name="portRequest">Port for request</param>
        public eClient(string hostname, int port, int portRequest = -1)
        {
            Hostname = hostname;
            Port = port;
            PortRequest = portRequest;

            Client = new TcpClient();

            if (PortRequest != -1)
            {
                ClientRequest = new TcpClient();
                ClientRequest.ReceiveTimeout = 1000;
            }
            else
                ClientRequest = null;

            TaskList = new List<Task>();

            Connected = false;
        }

        #endregion

        #region PublicMethodsAndFunctions

        /// <summary>
        /// Connect the client
        /// </summary>
        /// <returns>Success</returns>
        public bool Connect()
        {
            try
            {
                Client.Connect(Hostname, Port);

                if (ClientRequest != null)
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

                DebugMessage("Connected successfully");

                Connected = true;

                return true;
            }
            catch (Exception ex)
            {
                DebugMessage("Failed to connect : " + ex.Message);
                Disconnect();
                return false;
            }
        }

        /// <summary>
        /// Disconnect the client
        /// </summary>
        /// <returns>Success</returns>
        public bool Disconnect()
        {
            if (Connected)
            {
                try
                {
                    if (Client.Connected)
                        Client.Close();

                    if (ClientRequest != null)
                    {
                        if (ClientRequest.Connected)
                            ClientRequest.Close();
                    }

                    if (OnDisconnected != null)
                        OnDisconnected();

                    DebugMessage("Disconnected successfully");

                    Connected = false;

                    return true;

                }
                catch (Exception ex)
                {
                    DebugMessage("Failed to disconnect : " + ex.Message);
                    return false;
                }
            }
            return false;

        }

        /// <summary>
        /// Send a buffer
        /// </summary>
        /// <param name="buffer">Buffer to send</param>
        /// <returns>Success</returns>
        public bool Send(byte[] buffer)
        {
            try
            {
                byte[] b = eUtils.GetBuffer(buffer);
                Client.Send(b);
                return true;
            }
            catch (Exception ex)
            {
                DebugMessage("Failed to send buffer : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Send a request
        /// </summary>
        /// <param name="buffer">Buffer to send</param>
        /// <returns>Response</returns>
        public byte[] SendRequest(byte[] buffer)
        {
            if (ClientRequest != null)
            {
                try
                {
                    byte[] b = eUtils.GetBuffer(buffer);
                    ClientRequest.Send(b);

                    byte[] result, resultSize = new byte[2];
                    int size;


                    ClientRequest.GetStream().Read(resultSize, 0, resultSize.Length);

                    using (PacketReader pr = new PacketReader(resultSize))
                    {
                        size = pr.ReadInt16();
                    }

                    result = new byte[size];

                    ClientRequest.GetStream().Read(result, 0, result.Length);

                    return result;
                }
                catch (Exception ex)
                {
                    DebugMessage("Failed to send request : " + ex.Message);
                }
            }
            else
            {
                DebugMessage("This client cant send request");
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

        private void DebugMessage(object message)
        {
            if (Debug)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Debug] " + message);
                Console.ResetColor();
            }
        }

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
            byte[] buffer, bufferSize = new byte[2];
            int size;

            try
            {
                NetworkStream clientStream = Client.GetStream();
                while (Client.Connected)
                {
                    int bytesRead = await clientStream.ReadAsync(bufferSize, 0, bufferSize.Length);

                    if (bytesRead == 0)
                        break;

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
            catch(Exception ex)
            {
                //HResult == -2146232800 = Server closed
                if(Connected && ex.HResult != -2146232800)
                    DebugMessage("Failed to handle client : " + ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        #endregion
    }
}
