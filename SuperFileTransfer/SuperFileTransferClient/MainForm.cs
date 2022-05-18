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
        List<computer> computers;
        Random rnd = new Random();
        string lblguid;

        public MainForm()
        {
            InitializeComponent();
            computers = new List<computer>();
            thisClientId = Guid.NewGuid();
            lblguid = $"guid: {thisClientId}";
        }

        private void button1_Click(object senderIGNORE, EventArgs e)
        {
            byte[] guid = thisClientId.ToByteArray();
            label1.Text = lblguid;
            int port = StartListener();


            string server = "localhost";
            sender = new TcpClient();
            sender.Connect(server, 13531);
            senderStream = sender.GetStream();
            //server
            senderStream.write(guid);
            senderStream.WriteByte(1);
            senderStream.writeInt(port);


            retriever = new TcpClient();
            retriever.Connect(server, 13531);
            retrieverStream = retriever.GetStream();
            //client
            retrieverStream.write(guid);
            retrieverStream.WriteByte(0);
            var t = new Thread(ProcessRequest);
            t.IsBackground = true;
            t.Start();
        }


        private int StartListener()
        {
            TcpListener listener;
            int port;
            while (true)
            {
                port = rnd.Next(1, 65565);
                try
                {
                    listener = new TcpListener(IPAddress.Any, port);
                    listener.Start();

                    break;
                }
                catch (Exception)
                {
                     
                }
            }
            var t = new Thread(transferFiles);
            t.IsBackground = true;
            t.Start(listener);
            return port;
        }

        private void transferFiles(object o)
        {
            var listener = (TcpListener)o;
            while (true)
            {
                var client = listener.AcceptTcpClient();
                var ns = client.GetStream();
                var t = new Thread(processTransfer);
                t.IsBackground = true;
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
                var cmd = (CommandToClient)retrieverStream.ReadByte();
                if (cmd == CommandToClient.AddComputer)
                {
                    var guidBytes = retrieverStream.read(16);
                    var guid = new Guid(guidBytes);
                    var addrLen = retrieverStream.readInt();
                    var addrBytes = retrieverStream.read(addrLen);
                    var addr = new IPAddress(addrBytes);
                    var port = retrieverStream.readInt();
                    var hasAddress = retrieverStream.ReadByte() == 1;
                    var c = new computer(guid, new IPEndPoint(addr, port), hasAddress);
                    computers.Add(c);
                    this.Invoke(new Action(() =>
                    {
                        refreshComputersList();
                    }));
                }
            }
        }

        private void refreshComputersList()
        {
            throw new NotImplementedException();
        }
    }
}
