using Business.Exceptions;
using System.Collections.Generic;
using System;

namespace Business
{
    public class BusinessController
    {
        private IStore Store { get; set; }
        private Server Server { get; set; }
        private readonly object _locker = new object();

        public BusinessController(IStore store)
        {
            Store = store;
            Server = new Server();
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

        public void FriendshipRequest(Client sender, string receiverUsername)
        {
            Client receiver = Store.GetClient(receiverUsername);
            if (receiver == null)
                throw new RecordNotFoundException("The client doesn't exist");
            receiver.AddFriendshipRequest(sender);
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
            List<FriendshipRequest> requests = currentClient.FriendshipRequests;
            string[][] formattedRequests = new string[requests.Count][];
            for (int i = 0; i < requests.Count; i++)
            {
                formattedRequests[i] = new[] {requests[i].Id.ToString(), requests[i].Sender.Username};
            }
            return formattedRequests;
        }

        public List<Message> UnreadMessages(Client of, string from)
        {
            lock (_locker)
            {
                return Store.UnreadMessages(of, from);
            }
        }

        public List<string> GetNotificationsOf(Client loggedUser)
        {
            var notifications = new List<string>();
            Client storedClient = Store.GetClient(loggedUser.Username);

            string friendshipRequests = storedClient.FriendshipRequests.Count.ToString();

            notifications.Add(friendshipRequests);
            return notifications;
        }

        public List<Client> GetFriendsOf(Client client)
        {
            return Store.GetFriendsOf(client);
        }

        public FriendshipRequest ConfirmFriendshipRequest(Client currentClient, string requestId)
        {
            return currentClient.ConfirmRequest(requestId);
        }

        public void SendMessage(string usernameFrom, string usernameTo, string message)
        {
            lock (_locker)
            {
                Store.SendMessage(usernameFrom, usernameTo, message);
            }
        }
    }
}