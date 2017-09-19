using System.Collections.Generic;

namespace Business
{
    internal class Server
    {
        public IList<Client> ConnectedClients { get; set; }

        public Server()
        {
            ConnectedClients = new List<Client>();
        }

        public void ConnectClient(Client client)
        {
            ConnectedClients.Add(client);
        }
    }
}
