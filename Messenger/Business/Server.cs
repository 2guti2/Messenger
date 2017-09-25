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

        public Client GetLoggedClient(string token)
        {
            return ConnectedClients[token];
        }

        public bool IsClientConnected(Client client)
        {
            return ConnectedClients.ContainsValue(client);
        }

        public void DisconnectClient(string token)
        {
            ConnectedClients.Remove(token);
        }

        internal List<Client> GetLoggedClients()
        {
            var clients = new List<Client>();

            foreach(KeyValuePair<string, Client> keyValuePair in ConnectedClients)
            {
                clients.Add(keyValuePair.Value);
            }

            return clients;
        }
    }
}
