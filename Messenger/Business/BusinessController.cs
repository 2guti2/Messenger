using Business.Exceptions;
using System.Collections.Generic;
using System;

namespace Business
{
    public class BusinessController
    {
        private IStore Store { get; set; }
        private Server Server { get; set; }
        private readonly object messagesLocker = new object();
        private readonly object loginLocker = new object();
        private readonly object friendshipLocker = new object();

        public BusinessController(IStore store)
        {
            Store = store;
            Server = new Server();
        }

        public string Login(Client client)
        {
            lock (loginLocker)
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
        }

        public Client FriendshipRequest(Client sender, string receiverUsername)
        {
            lock (friendshipLocker)
            {
                Client receiver = Store.GetClient(receiverUsername);
                if (receiver == null) throw new RecordNotFoundException("The client doesn't exist");
                receiver.AddFriendshipRequest(sender);

                return receiver;
            }
        }

        public Client GetLoggedClient(string userToken)
        {
            lock (loginLocker)
            {
                Client loggedUser = Server.GetLoggedClient(userToken);
                if (loggedUser == null)
                    throw new ClientNotConnectedException();
                return loggedUser;
            }
        }

        public List<Client> GetLoggedClients()
        {
            lock (loginLocker)
            {
                return Server.GetLoggedClients();
            }
        }

        public void DisconnectClient(string token)
        {
            lock (loginLocker)
            {
                Server.DisconnectClient(token);
            }
        }

        public string[][] GetFriendshipRequests(Client currentClient)
        {
            lock (friendshipLocker)
            {
                List<FriendshipRequest> requests = currentClient.FriendshipRequests;
                var formattedRequests = new string[requests.Count][];
                for (var i = 0; i < requests.Count; i++)
                {
                    formattedRequests[i] = new[] {requests[i].Id.ToString(), requests[i].Sender.Username};
                }
                return formattedRequests;
            }
        }

        public List<Message> UnreadMessages(Client of, string from)
        {
            lock (messagesLocker)
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
            lock (friendshipLocker)
            {
                return Store.GetFriendsOf(client);
            }
        }

        public FriendshipRequest ConfirmFriendshipRequest(Client currentClient, string requestId)
        {
            lock (friendshipLocker)
            {
                return currentClient.ConfirmRequest(requestId);
            }
        }

        public void RejectFriendshipRequest(Client currentClient, string requestId)
        {
            lock (friendshipLocker)
            {
                currentClient.RejectRequest(requestId);
            }
        }

        public void SendMessage(string usernameFrom, string usernameTo, string message)
        {
            lock (messagesLocker)
            {
                Store.SendMessage(usernameFrom, usernameTo, message);
            }
        }

        public List<Message> AllMessages(Client loggedUser, string recipient)
        {
            lock (messagesLocker)
            {
                return Store.AllMessages(loggedUser, recipient);
            }
        }

        public List<Client> GetClients()
        {
            lock (loginLocker)
            {
                return Store.GetClients();
            }
        }
    }
}