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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperFileTransferClient
{
    public partial class ComputerControl : UserControl
    {

        private computer Computer;
        public ComputerControl(computer Computer)
        {
            InitializeComponent();
            this.Computer = Computer;
            var ipEndPoint = Computer.endPoint;
            lblAddr.Text = ipEndPoint.Address.ToString();
            lblPort.Text = ipEndPoint.Port.ToString();
            lblPort.BackColor = Computer.hasAccess ? Color.Green : Color.Red;
            
        }

        private void ComputerControl_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (fileList == null || fileList.Length <= 0)
            {
                return;
            }
            var client = new TcpClient();
            client.Connect(Computer.endPoint);
            var ns = client.GetStream();
            ns.WriteByte((byte)TransferCommands.Send);
            ns.writeInt(fileList.Length);
            foreach (var path in fileList)
            {
                ns.writeLong(new FileInfo(path).Length);
                ns.writeString(Path.GetFileName(path));
            }
            bool confirmation = ns.readBool();
            if (confirmation)
            {
                foreach (var path in fileList)
                {
                    using(Stream source = File.OpenRead(path)) 
                    {
                        byte[] buffer = new byte[2048];
                        int bytesRead;
                        while((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ns.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
            else
            {
                this.Invoke(new Action(() => {
                    MessageBox.Show("пользователь отказал или не смог принять файл(ы)");
                }));
            }
        }

        private void ComputerControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }
    }
}
