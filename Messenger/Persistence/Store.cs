using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Business;
using Business.Exceptions;

namespace Persistence
{
    public class Store : MarshalByRefObject, IStore
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

        public void ConnectClient(Client client, Session session)
        {
            Client storedClient = GetClient(client.Username);
            storedClient.ConnectionsCount++;
            storedClient.AddSession(session);
        }

        public void DisconnectClient(string token)
        {
            Client storedClient = Clients.Find(c => c.Sessions.Exists(s => s.Id.Equals(token)));
            storedClient.Sessions = new List<Session>();
        }

        public void UpdateClient(Client existingClient, Client newClient)
        {
            Client storedClient = GetClient(existingClient.Username);

            storedClient.Username = newClient.Username;
            storedClient.Password = newClient.Password;
        }

        public Client FriendshipRequest(Client sender, string receiverUsername)
        {
            Client receiver = GetClient(receiverUsername);
            Client storeSender = GetClient(sender.Username);
            if (receiver == null) throw new RecordNotFoundException("The client doesn't exist");
            receiver.AddFriendshipRequest(storeSender);

            return receiver;
        }

        public FriendshipRequest ConfirmFriendshipRequest(Client currentClient, string requestId)
        {
            Client storedClient = GetClient(currentClient.Username);
            return storedClient.ConfirmRequest(requestId);
        }

        public void RejectRequest(Client currentClient, string requestId)
        {
            Client storedClient = GetClient(currentClient.Username);
            storedClient.RejectRequest(requestId);
        }
    }
}
