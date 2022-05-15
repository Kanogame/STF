using FIleTransferCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SuperFileTransfer
{
    public class computer
    {
        Guid guid;
        TcpClient toServer;
        TcpClient toClient;
        bool TransferPortWorks;

        public computer(Guid guid)
        {
            this.guid = guid;
        }

        public void SetChannelToServer(TcpClient chan)
        {
             this.toServer = chan;
        }

        public void SetChannelToClient(TcpClient chan)
        {
            this.toClient = chan;
        }

        public bool BothChannelIsExist()
        {
            return toServer != null && toClient != null;
        }

        public void CheckTransferPort()
        {
            var cli = new TcpClient();
            var ep = (IPEndPoint)toServer.Client.RemoteEndPoint;
            try
            {
                cli.Connect(ep.Address, 13532);
                var ns = cli.GetStream();
                ns.WriteByte((byte)TransferCommands.Ping);
                Guid retrieverGuid = new Guid(ns.read(16));
                if (retrieverGuid == guid)
                {
                    TransferPortWorks = true;
                }
                else
                {
                     TransferPortWorks = false;
                }
                cli.Close();
            }catch (Exception)
            {
                 TransferPortWorks = false;
            }
        }
    }
}
