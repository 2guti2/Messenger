using Business.Exceptions;
using System.Collections.Generic;

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
            if (isValidPassword && !isClientConnected)
            {
                return Server.ConnectClient(client);
            }
            if (!isValidPassword)
            {
                return "";
            }
            throw new ClientAlreadyConnectedException();
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

        public FriendshipRequest ConfirmFriendshipRequest(Client currentClient, string requestId)
        {
            return currentClient.ConfirmRequest(requestId);
        }
    }
}