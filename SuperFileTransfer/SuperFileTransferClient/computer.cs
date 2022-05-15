using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SuperFileTransferClient
{
    public class computer
    {
        public Guid guid;
        public IPEndPoint endPoint;
        public bool hasAccess;
            
        public computer(Guid giud, IPEndPoint endPoint, bool hasAccess)
        {
            this.guid = guid;
            this.endPoint = endPoint;
            this.hasAccess = hasAccess;
        }
    }
}
