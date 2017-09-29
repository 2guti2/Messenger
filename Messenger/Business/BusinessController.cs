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

        public Client FriendshipRequest(Client sender, string receiverUsername)
        {
            Client receiver = Store.GetClient(receiverUsername);
            if (receiver == null)
                throw new RecordNotFoundException("The client doesn't exist");
            receiver.AddFriendshipRequest(sender);

            return receiver;
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
            var formattedRequests = new string[requests.Count][];
            for (var i = 0; i < requests.Count; i++)
            {
                formattedRequests[i] = new[] {requests[i].Id.ToString(), requests[i].Sender.Username};
            }
            return formattedRequests;
        }

        public List<Client> GetFriendsOf(Client client)
        {
            return Store.GetFriendsOf(client);
        }

        public FriendshipRequest ConfirmFriendshipRequest(Client currentClient, string requestId)
        {
            return currentClient.ConfirmRequest(requestId);
        }

        public void RejectFriendshipRequest(Client currentClient, string requestId)
        {
            currentClient.RejectRequest(requestId);
        }
        
        public List<Client> GetClients()
        {
            return  Store.GetClients();
        }
    }
}