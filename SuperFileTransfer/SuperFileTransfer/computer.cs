using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SuperFileTransfer
{
    public class computer
    {
        string id;
        TcpClient toServer;
        TcpClient toClient;

        public computer(string id)
        {
            this.id = id;
        }

        public void SetChannelToServer(TcpClient chan)
        {
             this.toServer = chan;
        }

        public void SetChannelToClient(TcpClient chan)
        {
            this.toClient = chan;
        }
    }
}
