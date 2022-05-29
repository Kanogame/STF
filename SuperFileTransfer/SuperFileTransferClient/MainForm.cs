using FIleTransferCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
                var t = new Thread(processTransfer);
                t.IsBackground = true;
                t.Start(client);
            }
        }

        private void processTransfer(object o)
        {
            var client = (TcpClient)o;
            var ns = client.GetStream();
            var command = (TransferCommands)ns.ReadByte();
            if (command == TransferCommands.Ping)
            {
                 ns.write(thisClientId.ToByteArray());
            } else if (command == TransferCommands.Send)
            {
                int fileCount = ns.readInt();
                long[] lengthArray = new long[fileCount];
                string[] NameArray = new string[fileCount];
                string fileDescriptions = "";
                for (int i = 0; i < fileCount; i++)
                {
                    lengthArray[i] = ns.readLong();
                    NameArray[i] = ns.readString();
                    fileDescriptions =$"{NameArray[i]}, {lengthArray[i]} байт\n";
                }
                string question = $"вы желаете принять {fileCount} файлов от {client.Client.RemoteEndPoint}: \n" + fileDescriptions;
                bool confirmation = MessageBox.Show(question, "принять", MessageBoxButtons.YesNoCancel) == DialogResult.Yes;
                if (confirmation)
                {
                    string folderPath = "";
                    this.Invoke(new Action(() => {
                    using (var folderDialog = new FolderBrowserDialog())
                    {
                        folderDialog.Description = "в какую папку вы желаете сожранить файлы?";
                        confirmation = folderDialog.ShowDialog() == DialogResult.OK;
                        folderPath = folderDialog.SelectedPath;
                    }
                    }));
                    for (int i = 0; i < 0; i++)
                    {
                        string path = Path.Combine(folderPath, NameArray[i]);
                        if (File.Exists(path))
                        {
                            confirmation = false;
                            MessageBox.Show($"файл уже существует: {path}");
                            break;
                        }
                    }
                    ns.writeBool(confirmation);
                    if (confirmation)
                    {
                        for (int i = 0; i < fileCount; i++)
                        {
                            long left = lengthArray[i];
                            string path = Path.Combine(folderPath, NameArray[i]);
                            using (Stream f = File.OpenWrite(path))
                            {
                                while (left > 0)
                                {
                                    int cnt = (int)Math.Min(left, 2048);
                                    byte[] bytes = ns.read(cnt);
                                    f.Write(bytes, 0, bytes.Length);
                                    left -= cnt;
                                }
                            }
                        }
                    }
                }
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
                this.Invoke(new Action(() => {
                    button1.Visible = true;
                    button1.Enabled = true; 
                    label3.Visible = false; 
                    label4.Visible = false; 
                    computerControl.Clear(); 
                    panel.Controls.Clear();
                    } ));
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
