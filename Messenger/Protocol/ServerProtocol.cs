using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using Business;

namespace Protocol
{
    public class ServerProtocol
    {
        private Socket Socket { get; set; }
        private const int TokenLength = 8;
        private Dictionary<Client, string> tokenDictionary;

        public ServerProtocol()
        {
            tokenDictionary = new Dictionary<Client, string>();
        }

        public void Start(string ip, int port)
        {
            var serverIpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            Socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            Socket.Bind(serverIpEndPoint);
            Socket.Listen(100);
        }

        public delegate void ConnectionDelegate(Connection connection);

        public void ReceiveMessage(ConnectionDelegate onConnection)
        {
            if (onConnection == null) throw new ArgumentNullException(nameof(onConnection));

            var clientSocket = Socket.Accept();
            var thread = new Thread(() => onConnection(new Connection(clientSocket)));
            thread.Start();
        }

        public static string GenerateRandomToken()
        {
            RNGCryptoServiceProvider cryptRNG = new RNGCryptoServiceProvider();
            byte[] tokenBuffer = new byte[TokenLength];
            cryptRNG.GetBytes(tokenBuffer);
            return Convert.ToBase64String(tokenBuffer);
        }
    }
}
