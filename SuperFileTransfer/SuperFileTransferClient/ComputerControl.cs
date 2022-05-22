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
            
        }
    }
}
