using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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

        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object senderIGNORE, EventArgs e)
        {
            string server = "localhost";
            sender = new TcpClient();
            sender.Connect(server, 13531);
            senderStream = sender.GetStream();
            senderStream.WriteByte(1);


            retriever = new TcpClient();
            retriever.Connect(server, 13531);
            retrieverStream = retriever.GetStream();
            retrieverStream.WriteByte(0);
        }
    }
}
