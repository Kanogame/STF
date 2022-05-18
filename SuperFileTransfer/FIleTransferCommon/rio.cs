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

        public static void writeInt(this NetworkStream stream, int val)
        {
            byte[] byteArray = BitConverter.GetBytes(val);
             stream.Write(byteArray, 0, byteArray.Length);
        }

        public static int readInt(this NetworkStream stream)
        {
             byte[] byteArray = stream.read(4);
             return BitConverter.ToInt32(byteArray, 0);
        }

        public static void writeString(this NetworkStream stream, string val)
        {
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(val);
            stream.writeInt(bytes.Length);
            stream.write(bytes);
        }

        public static string readString(this NetworkStream stream)
        {
            int len = stream.readInt();
            var bytes = stream.read(len);
            return UTF8Encoding.UTF8.GetString(bytes);
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
