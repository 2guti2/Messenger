using System;
using System.Collections.Generic;
using System.Linq;

namespace Business
{
    [Serializable]
    internal class Server
    {
        private IStore store;

        public List<Session> ConnectedClients { get; set; }

        public Server(IStore store)
        {
            this.store = store;
            ConnectedClients = new List<Session>();
        }

        public string ConnectClient(Client client)
        {
            var session = new Session(client);
            ConnectedClients.Add(session);

            store.ConnectClient(client, session);

            return session.Id;
        }

        public Client GetLoggedClient(string token)
        {
            return store.GetClient(ConnectedClients.Find(session => session.Id.Equals(token))?.Client.Username);
        }

        public bool IsClientConnected(Client client)
        {
            return ConnectedClients.Exists(session => session.Client.Equals(client));
        }

        public void DisconnectClient(string token)
        {
            ConnectedClients.FindAll(session => session.Id.Equals(token)).ForEach(sesssion => sesssion.Deactivate());
            ConnectedClients.RemoveAll(session => session.Id.Equals(token));

            store.DisconnectClient(token);
        }

        public List<Client> GetLoggedClients()
        {
            List<Client> storedClients = store.GetClients();

            List<Client> connectedClients = new List<Client>();

            foreach (Client storedClient in storedClients)
            {
                if(storedClient.Sessions.Count > 0)
                    connectedClients.Add(storedClient);
            }

            return storedClients;
        }
    }
}