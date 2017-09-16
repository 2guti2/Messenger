using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Protocol
{
    public class ServerProtocol : Protocol
    {
        private Socket socket;

        public void Start(string ip, int port)
        {
            var serverIpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            socket.Bind(serverIpEndPoint);
            socket.Listen(100);
        }

        public void AcceptRequest()
        {
            var clientSocket = socket.Accept();
            var thread = new Thread(() => HandleClient(clientSocket));
            thread.Start();
        }

        private void HandleClient(Socket clientSocket)
        {
            ReadData(clientSocket);
            Console.WriteLine("Client handled");
        }
    }
}
