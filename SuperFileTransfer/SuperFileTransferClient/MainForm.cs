using FIleTransferCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperFileTransferClient
{
    public partial class MainForm : Form
    {

        TcpClient sender;
        TcpClient retriever;
        NetworkStream senderStream;
        NetworkStream retrieverStream;
        Guid thisClientId;

        public MainForm()
        {
            InitializeComponent();
            thisClientId = Guid.NewGuid();
        }

        private void button1_Click(object senderIGNORE, EventArgs e)
        {
            byte[] guid = thisClientId.ToByteArray();
            StartListener();

            string server = "localhost";
            sender = new TcpClient();
            sender.Connect(server, 13531);
            senderStream = sender.GetStream();
            //server
            senderStream.write(guid);
            senderStream.WriteByte(1);

            retriever = new TcpClient();
            retriever.Connect(server, 13531);
            retrieverStream = retriever.GetStream();
            //client
            retrieverStream.WriteByte(0);
            var t = new Thread(ProcessRequest);
            t.Start();
        }


        private void StartListener()
        {
            var listener = new TcpListener(IPAddress.Any, 13531);
            listener.Start();
            var t = new Thread(transferFiles);
            t.Start();
        }

        private void transferFiles(object o)
        {
            var listener = (TcpListener)o;
            while (true)
            {
                var client = listener.AcceptTcpClient();
                var ns = client.GetStream();
                var t = new Thread(processTransfer);
                t.Start();
            }
        }

        private void processTransfer(object o)
        {
             var ns = (NetworkStream)o;
            var command = (TransferCommands)ns.ReadByte();
            if (command == TransferCommands.Ping)
            {
                 ns.WriteByte((byte)TransferCommands.Ping);
            }
        }

        private void ProcessRequest()
        {
            while (true)
            {

            }
        }
    }
}
