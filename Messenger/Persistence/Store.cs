using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Business;
using Business.Exceptions;

namespace Persistence
{
    public class Store : IStore
    {
        private List<Client> Clients { get; set; }

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

            clientFrom.Messages.Add(message);
            clientTo.Messages.Add(message);
        }

        public List<Message> UnreadMessages(Client of, string from)
        {
            var messages = new List<Message>();

            Client clientOf = Clients.Find(c => c.Equals(of));

            if (clientOf == null) return messages;
            messages.AddRange(clientOf.Messages.Where(message => message.Sender.Equals(from) && !message.Read));
            messages.ForEach(message => message.Read = true);

            return messages;
        }

        public List<Message> AllMessages(Client of, string from)
        {
            var messages = new List<Message>();

            Client clientOf = Clients.Find(c => c.Equals(of));

            if (clientOf == null) throw new RecordNotFoundException("Client was not found");

            messages.AddRange
            (
                clientOf.Messages.Where(message => ((message.Receiver.Equals(from) || message.Sender.Equals(from))))
            );
            messages.ForEach(m => m.Read = true);

            return messages;
        }

        public List<Client> GetClients()
        {
            return Clients;
        }

        public void DeleteClient(Client client)
        {
            Clients.Remove(client);
        }
    }
}
