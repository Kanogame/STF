using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
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
            for (int i = 0; i < fileList.Length; i++)
            {
                var path = fileList[i];
                MessageBox.Show(path);
            }
        }

        private void ComputerControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }
    }
}
