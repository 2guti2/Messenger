using System;
using System.Collections.Generic;
using Business;

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

        public Client GetClient(string clientUsername)
        {
            return Clients.Find(client => client.Username.Equals(clientUsername));
        }

        public List<Client> GetFriendsOf(Client client)
        {
            return Clients.Find(c => c.Equals(client)).Friends;
        }

        public void SendMessage(string usernameFrom, string usernameTo, string messageContent)
        {
            Client clientFrom = Clients.Find(c => c.Username.Equals(usernameFrom));
            Client clientTo = Clients.Find(c => c.Username.Equals(usernameTo));

            var message = new Message()
            {
                Sender = usernameFrom,
                Receiver = usernameTo,
                Content = messageContent,
                TimeStamp = DateTime.Now,
                Read = false
            };

            Clients.Remove(clientFrom);
            Clients.Remove(clientTo);

            clientFrom.Messages.Add(message);
            clientTo.Messages.Add(message);

            Clients.Add(clientFrom);
            Clients.Add(clientTo);
        }

        public List<Message> UnreadMessages(Client of, string from)
        {
            var messages = new List<Message>();

            Client clientOf = Clients.Find(c => c.Username.Equals(of));

            if(clientOf != null)
            foreach(Message m in clientOf.Messages)
                if (m.Sender.Equals(from) && !m.Read)
                    messages.Add(m);

            return messages;
        }
    }
}
