using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Business
{
    internal class Server
    {
        private Dictionary<string, Client> ConnectedClients { get; set; }

        public Server()
        {
            ConnectedClients = new Dictionary<string, Client>();
        }

        public string ConnectClient(Client client)
        {
            string token = GenerateRandomToken();
            ConnectedClients.Add(token, client);

            return token;
        }

        private static string GenerateRandomToken(int length = 12)
        {
            var cryptRng = new RNGCryptoServiceProvider();
            var tokenBuffer = new byte[length];
            cryptRng.GetBytes(tokenBuffer);
            return Convert.ToBase64String(tokenBuffer);
        }
    }
}
