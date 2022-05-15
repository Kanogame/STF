using FIleTransferCommon;
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

        Dictionary<string, computer> computers;

        static void Main(string[] args)
        {
            var p = new Program();
            p.main();

        }

        Program()
        {
            computers = new Dictionary<string, computer>();
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
            var clientGuidBytes = ns.read(16);
            var clientGuid = new Guid(clientGuidBytes);
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
            addChannel(clientGuid, client, requestsToServer);
        }

        void addChannel(Guid clientGuid, TcpClient client, bool requestsToServer)
        {
            var clientEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            string clientId = $"{clientGuid}:{clientEndPoint.Address}";
            Console.WriteLine(clientId);
            computer comp;
            if (computers.ContainsKey(clientId))
            {
                comp = computers[clientId];
            }
            else
            {
                 comp = new computer(clientGuid);
                 computers.Add(clientId, comp);
            }
            if (requestsToServer)
            {
                comp.SetChannelToClient(client);
            }
            else
            {
                 comp.SetChannelToClient(client);
            }
            if (comp.BothChannelIsExist())
            {
                comp.CheckTransferPort();
            }
        }

        void SignalToClients()
        {

        }
    }
}
