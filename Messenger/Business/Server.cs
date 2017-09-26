using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<Client> GetLoggedClients()
        {
            return ConnectedClients.Select(keyValuePair => keyValuePair.Value).ToList();
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