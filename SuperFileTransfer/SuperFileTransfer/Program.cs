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
        ProducerConsumerQueue<TaskToClient> queueToClient;

        static void Main(string[] args)
        {
            var p = new Program();
            p.main();

        }

        Program()
        {
            computers = new Dictionary<string, computer>();
            queueToClient = new ProducerConsumerQueue<TaskToClient>(processTaskToClient);
        }

        void processTaskToClient(TaskToClient Task)
        {
            //invoke in other thread
            if (Task is TaskAddComputer)
            {
                var t = (TaskAddComputer)Task;
                var ns = t.SendTo.getStreamToClient();
                ns.WriteByte((byte)CommandToClient.AddComputer);
                var who = t.whoAdded;
                ns.writeGuid(who.getGuid());
                ns.writeIPAddress(who.getIPAddress());
                ns.writeInt(who.GetPortForFileTransfer());
                ns.writeBool(who.getHasAccess());
                
            }
        }

        void main()
        {
            var acceptClientThreads = new Thread(AcceptClients);
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
            lock (computers)
            {
                if (computers.ContainsKey(clientId))
                {
                    comp = computers[clientId];
                }
                else
                {
                     comp = new computer(clientGuid, clientId);
                     computers.Add(clientId, comp);
                }
                if (requestsToServer)
                {
                    comp.SetChannelToServer(client);
                    int port = client.GetStream().readInt();
                    comp.SetPortForFileTranfer(port);
                }
                else
                {
                     comp.SetChannelToClient(client);
                }
                if (comp.BothChannelIsExist())
                {
                    comp.CheckTransferPort();
                    notifyConputerAdded(comp);
                }
            }
        }

        void notifyConputerAdded(computer comp)
        {
            foreach (var current in computers)
            {
                 if (current.Value == comp)
                 {
                     continue;
                 }
                 if (current.Value.BothChannelIsExist())
                 {
                     var t = new TaskAddComputer(current.Value, comp);
                     queueToClient.EnqueueTask(t);
                 }
            }
        }
    }
}
