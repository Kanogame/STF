using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FIleTransferCommon
{
    public static class rio
    {
        public static void write(this NetworkStream stream, byte[] bytes)
        {
             stream.Write(bytes, 0, bytes.Length);
        }

        public static byte[] read(this NetworkStream stream, int bytesCount, Action<int> progress = null)
        {
            int left = bytesCount;
            int ind = 0;
            byte[] buffer = new byte[bytesCount];
            while (left > 0)
            {
                int cnt = stream.Read(buffer, ind, left);
                left -= cnt;
                ind += cnt;
                if (progress != null)
                {
                    progress(left);
                }
            }
            return buffer;
        }
    }
}
