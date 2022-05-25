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
        List<ComputerControl> computerControl;                          
        Random rnd = new Random();                                      
        string lblguid;                                                 
                                                                        
        public MainForm()                                               
        {                                                               
            InitializeComponent();                                      
            computers = new List<computer>();                           
            computerControl = new List<ComputerControl>();              
            thisClientId = Guid.NewGuid();                              
            lblguid += $"guid: {thisClientId}";
        }

        private string getLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("no network adapter");
        }

        private void button1_Click(object senderIGNORE, EventArgs e)
        {
            button1.Enabled = false;
            button1.Visible = false;
            byte[] guid = thisClientId.ToByteArray();
            label1.Text = lblguid;
            int port = StartListener();

            label3.Visible = true;
            string IPAddr = getLocalIPAddress();
            label3.Text += IPAddr;

            label4.Visible = true;
            label4.Text += port;


            string server = textBox1.Text;
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
                     //ignore
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
                t.Start(ns);
            }
        }

        private void processTransfer(object o)
        {
             var ns = (NetworkStream)o;
            var command = (TransferCommands)ns.ReadByte();
            if (command == TransferCommands.Ping)
            {
                 ns.write(thisClientId.ToByteArray());
            }
        }

        private void ProcessRequest()
        {
            try
            {
                while (true)
                {
                    var cmd = (CommandToClient)retrieverStream.ReadByte();
                    if (cmd == CommandToClient.AddComputer)
                    {
                        var guid = retrieverStream.readGuid();
                        var addr = retrieverStream.readIpAddress();
                        var port = retrieverStream.readInt();
                        var hasAddress = retrieverStream.readBool();
                        var c = new computer(guid, new IPEndPoint(addr, port), hasAddress);
                        computers.Add(c);
                        this.Invoke(new Action(() =>
                        {
                           addComputerControl(c);
                        }));
                    }
                    else if (cmd == CommandToClient.RemoveComputer)
                    {
                        var guid = retrieverStream.readGuid();
                        var ind = - 1;
                        for (int i = 0; i < computers.Count; i++)
                        {
                            if (computers[i].guid == guid)
                            {
                                ind = i;
                                break;
                            }
                        }
                        if (ind > -1)
                        {
                            computers.RemoveAt(ind);
                            this.Invoke(new Action(() =>
                            {
                                removeComputerControl(ind);
                            }));
                        }
                        computers = computers.Where(x => x.guid != guid).ToList();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("вы не нравитесь серваку, он ушел");
                this.Invoke(new Action(() => {button1.Visible = true; } ));
                this.Invoke(new Action(() => {button1.Enabled = true; } ));
                this.Invoke(new Action(() => {label3.Visible = false; } ));
                this.Invoke(new Action(() => {label4.Visible = false; } ));
                this.Invoke(new Action(() => {computerControl.Clear(); } ));
                this.Invoke(new Action(() => {panel.Controls.Clear(); } ));
            }
        }

        private void addComputerControl(computer c)
        {
            var ctrl = new ComputerControl(c);
            panel.Controls.Add(ctrl);
            computerControl.Add(ctrl);
            setComputerControlsLocation();
        }

        private void removeComputerControl(int ind)
        {
            var ctrl = computerControl[ind];
            computerControl.RemoveAt(ind);
            panel.Controls.RemoveAt(ind);
            ctrl.Dispose();
            setComputerControlsLocation();
        }

        private void setComputerControlsLocation()
        {
            if (computerControl.Count <= 0)
            {
                return;
            }
            var cc = computerControl[0];
            var cw = cc.Width;
            var ch = cc.Height;
            int gap = 10;
            var cs = panel.ClientSize;
            var w = cs.Width;
            var h = cs.Height;
            var n = (w - gap) / (cw + gap);
            for (int i = 0; i < computerControl.Count; i++)
            {
                int rowIndex = i / n;
                int colIndex = i % n;
                var current = computerControl[i];
                current.Left = gap + colIndex * (cw + gap);
                current.Top = gap + rowIndex* (ch + gap);
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            setComputerControlsLocation();
        }
    }
}
