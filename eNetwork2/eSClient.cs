using System.Net.Sockets;

namespace eNetwork2
{
    /// <summary>
    /// eSClient, client server side
    /// </summary>
    public class eSClient
    {

        #region Variables

        private int ID;
        private TcpClient TcpClient;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor with ID and TcpClient
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="tcpClient">Client</param>
        public eSClient(int id, TcpClient tcpClient)
        {
            ID = id;
            TcpClient = tcpClient;
        }

        #endregion

        #region PublicMethodsAndFunctions

        /// <summary>
        /// Get the TcpClient
        /// </summary>
        /// <returns>Client</returns>
        public TcpClient GetTcpClient()
        {
            return TcpClient;
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

    }
}
