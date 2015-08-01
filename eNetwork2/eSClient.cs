using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace eNetwork2
{
    public class eSClient
    {

        //Fields

        private int ID;
        private TcpClient TcpClient;

        //Constructors

        public eSClient(int id, TcpClient tcpClient)
        {
            ID = id;
            TcpClient = tcpClient;
        }

        //Methods and functions

        public TcpClient GetTcpClient()
        {
            return TcpClient;
        }

        public int GetID()
        {
            return ID;
        }

    }
}
