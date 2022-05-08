using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SuperFileTransfer
{
    class Program
    {

        List<computer> computers;

        static void Main(string[] args)
        {
            var p = new Program();
            p.main();

        }

        Program()
        {
            computers = new List<computer>();
        }

        void main()
        {
            var acceptClientThreads = new Thread(AcceptClients);
            var signalsToClientsThread = new Thread(SignalToClients);
            acceptClientThreads.IsBackground = true;
            acceptClientThreads.Start();
            Console.ReadKey();
        }

        void AcceptClients()
        {
            var listener = new TcpListener(IPAddress.Any, 13531);
            listener.Start();
            while (true)
            {
                var client = listener.AcceptTcpClient();
                addClient(client);
            }
        }

        void addClient(TcpClient client)
        {
            var ns = client.GetStream();
            bool requestsToServer = ns.ReadByte() == 1;
            Console.WriteLine(client.Client.RemoteEndPoint.ToString());
            if (requestsToServer)
            {
                Console.WriteLine("Запросы на сервер");
            }
            else
            {
                Console.WriteLine("Запросы к клиенту");
            }
        }

        void SignalToClients()
        {

        }
    }
}
