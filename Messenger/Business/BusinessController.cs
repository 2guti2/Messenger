using Business.Exceptions;
using System.Collections.Generic;
using System;

namespace Business
{
    public class BusinessController
    {
        private IStore Store { get; set; }
        private Server Server { get; set; }

        public BusinessController(IStore store)
        {
            Store = store;
            Server = new Server(store);
        }

        public string Login(Client client)
        {
            if (!Store.ClientExists(client))
                Store.AddClient(client);
            Client storedClient = Store.GetClient(client.Username);
            bool isValidPassword = storedClient.ValidatePassword(client.Password);
            bool isClientConnected = Server.IsClientConnected(client);
            if (isValidPassword && isClientConnected)
                throw new ClientAlreadyConnectedException();

            return isValidPassword ? Server.ConnectClient(storedClient) : "";

        }

        public bool DeleteClient(Client client)
        {
            if (!Store.ClientExists(client))
                return false;

            Store.DeleteClient(client);

            return true;
        }

        public bool UpdateClient(Client existingClient, Client newClient)
        {
            if (!Store.ClientExists(existingClient))
                return false;

            Store.UpdateClient(existingClient, newClient);

            return true;
        }

        public void AddLogEntry(LogEntry entry)
        {
            Store.AddLogEntry(entry);
        }

        public List<LogEntry> GetLogEntries()
        {
            return Store.GetLogEntries();
        }

        public LogEntry GetLastLogEntry()
        {
            return Store.GetLastLogEntry();
        }

        public bool CreateClient(Client client)
        {
            if (!Store.ClientExists(client))
                Store.AddClient(client);
            else
                return false;

            return true;
        }

        public Client FriendshipRequest(Client sender, string receiverUsername)
        {
            return Store.FriendshipRequest(sender, receiverUsername);
        }

        public Client GetLoggedClient(string userToken)
        {
            Client loggedUser = Server.GetLoggedClient(userToken);
            if (loggedUser == null)
                throw new ClientNotConnectedException();
            return loggedUser;
        }

        public List<Client> GetLoggedClients()
        {
            return Server.GetLoggedClients();
        }

        public void DisconnectClient(string token)
        {
            Server.DisconnectClient(token);
        }

        public string[][] GetFriendshipRequests(Client currentClient)
        {
            List<FriendshipRequest> requests = Store.GetClient(currentClient.Username).FriendshipRequests;
            var formattedRequests = new string[requests.Count][];
            for (var i = 0; i < requests.Count; i++)
            {
                formattedRequests[i] = new[] { requests[i].Id.ToString(), requests[i].Sender.Username };
            }
            return formattedRequests;
        }

        public List<Message> UnreadMessages(Client of, string from)
        {
            return Store.UnreadMessages(of, from);
        }

        public List<Client> GetFriendsOf(Client client)
        {
            return Store.GetFriendsOf(client);
        }

        public FriendshipRequest ConfirmFriendshipRequest(Client currentClient, string requestId)
        {
            return Store.ConfirmFriendshipRequest(currentClient, requestId);
        }

        public void RejectFriendshipRequest(Client currentClient, string requestId)
        {
            Store.RejectRequest(currentClient, requestId);
        }

        public void SendMessage(string usernameFrom, string usernameTo, string message)
        {
            Store.SendMessage(usernameFrom, usernameTo, message);
        }

        public List<Message> AllMessages(Client loggedUser, string recipient)
        {
            return Store.AllMessages(loggedUser, recipient);
        }

        public List<Client> GetClients()
        {
            return Store.GetClients();
        }
    }
}