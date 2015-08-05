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

        public bool Debug { get; set; } = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="port">Port</param>
        /// <param name="portRequest">Port for request</param>
        public eServer(int port, int portRequest = -1)
        {
            Port = port;
            PortRequest = portRequest;
            Listener = new TcpListener(IPAddress.Any, Port);

            if (PortRequest != -1)
                ListenerRequest = new TcpListener(IPAddress.Any, PortRequest);
            else
                ListenerRequest = null;

            TaskList = new List<Task>();
            ClientList = new List<eSClient>();

            randomID = new Random();
        }

        #endregion

        #region PublicMethodsAndFunctions

        /// <summary>
        /// Start the server
        /// </summary>
        /// <returns>Success</returns>
        public bool Start()
        {
            try
            {
                //Start listen for client
                Listener.Start();
                StartListen();

                //Start listen for request
                if (ListenerRequest != null)
                    ListenerRequest.Start();
                if (ListenerRequest != null)
                    StartListenRequest();

                DebugMessage("Server started on port " + Port + (PortRequest != -1 ? " and request port " + PortRequest : "") + ".");
                return true;
            }
            catch (Exception ex)
            {
                DebugMessage("Failed to start the server : " + ex.Message);
                Stop();
                return false;
            }
        }

        /// <summary>
        /// Stop the server
        /// </summary>
        /// <returns>Success</returns>
        public bool Stop()
        {
            try
            {
                TaskList.Clear();
                ClientList.Clear();

                //Stop listen for client
                if (Listener.Server.Connected)
                    Listener.Stop();

                //Stop listen for request
                if (ListenerRequest != null)
                {
                    if (ListenerRequest.Server.Connected)
                        ListenerRequest.Stop();
                }

                DebugMessage("Server stopped.");
                return true;
            }
            catch (Exception ex)
            {
                DebugMessage("Failed to stop the server : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Send buffer to one client
        /// </summary>
        /// <param name="buffer">Buffer to send</param>
        /// <param name="client">Client</param>
        /// <returns>Success</returns>
        public bool SendTo(byte[] buffer, eSClient client)
        {
            try
            {
                byte[] b = eUtils.GetBuffer(buffer);
                client.GetTcpClient().Send(b);

                DebugMessage("Buffer sent successfully.");

                return true;
            }
            catch (Exception ex)
            {
                DebugMessage("Failed to send buffer : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Send buffer to one client (Request)
        /// </summary>
        /// <param name="buffer">Buffer to send</param>
        /// <param name="client">Client</param>
        /// <returns>Success</returns>
        public bool SendTo(byte[] buffer, TcpClient client)
        {
            try
            {
                byte[] b = eUtils.GetBuffer(buffer);
                client.Send(b);

                DebugMessage("Buffer sent successfully.");

                return true;
            }
            catch (Exception ex)
            {
                DebugMessage("Failed to send buffer : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Send buffer to all client connected
        /// </summary>
        /// <param name="buffer">Buffer to send</param>
        /// <returns>Success</returns>
        public bool SendToAll(byte[] buffer)
        {
            try
            {
                foreach (eSClient client in ClientList)
                {
                    byte[] b = eUtils.GetBuffer(buffer);
                    client.GetTcpClient().Send(b);
                }

                DebugMessage("Buffer sent successfully.");

                return true;
            }
            catch (Exception ex)
            {
                DebugMessage("Failed to send buffer : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Send buffer to all client connected except one
        /// </summary>
        /// <param name="buffer">Buffer to send</param>
        /// <param name="exceptedClient">Excepted client</param>
        /// <returns>Success</returns>
        public bool SendToAllExcept(byte[] buffer, eSClient exceptedClient)
        {
            try
            {
                foreach (eSClient client in ClientList)
                {
                    if (client != exceptedClient)
                    {
                        byte[] b = eUtils.GetBuffer(buffer);
                        client.GetTcpClient().Send(b);
                    }
                }
                DebugMessage("Buffer sent successfully.");

                return true;
            }
            catch (Exception ex)
            {
                DebugMessage("Failed to send buffer : " + ex.Message);
                return false;
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
        /// Show a debug message on console
        /// </summary>
        /// <param name="message"></param>
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
            try
            {
                while (true)
                {

                    TcpClient client = await Listener.AcceptTcpClientAsync();
                    Int32 id = (Int32)randomID.Next(10000, 99999);
                    eSClient sClient = new eSClient(id, client);

                    byte[] idBuffer;

                    using (PacketWriter pw = new PacketWriter())
                    {
                        pw.WriteInt32(id);
                        idBuffer = pw.ToArray();
                    }

                    client.Send(idBuffer);

                    ClientList.Add(sClient);
                    if (OnClientConnected != null)
                        OnClientConnected(sClient);
                    StartHandleClient(sClient);
                }
            }
            catch (Exception ex)
            {
                DebugMessage("Failed to listen for new client : " + ex.Message);
            }
            finally
            {
                Stop();
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
            byte[] buffer, bufferSize = new byte[2];
            int size;

            try
            {
                NetworkStream clientStream = client.GetTcpClient().GetStream();

                while (client.GetTcpClient().Client.Connected)
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
                        OnDataReceived(client, buffer);
                }
            }
            catch (Exception ex)
            {
                DebugMessage("Failed to handle client : " + ex.Message);
            }
            finally
            {
                ClientList.Remove(client);
                if (OnClientDisconnected != null)
                    OnClientDisconnected(client);
            }
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
            try
            {
                while (true)
                {
                    TcpClient client = await this.ListenerRequest.AcceptTcpClientAsync();
                    StartHandleClientRequest(client);
                }
            }
            catch (Exception ex)
            {
                DebugMessage("Failed to listen request for new client : " + ex.Message);
            }
            finally
            {
                Stop();
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
            byte[] buffer, bufferSize = new byte[2];
            int size;

            try
            {
                NetworkStream clientStream = client.GetStream();
                while (client.Client.Connected)
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

                    if (OnRequestReceived != null)
                        OnRequestReceived(client, buffer);
                }
            }
            catch (Exception ex)
            {
                DebugMessage("Failed to handle client request : " + ex.Message);
            }
        }

        #endregion
    }
}
