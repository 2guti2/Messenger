using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Business;
using System.Threading.Tasks;

namespace Persistence
{
    public class Store : IStore
    {
        public List<Client> Clients { get; set; }

        public Store()
        {
            Clients = new List<Client>();
        }

        public bool ClientExists(Client client)
        {
           return Clients.Contains(client);
        }

        public void AddClient(Client client)
        {
            Clients.Add(client);
        }
    }
}
