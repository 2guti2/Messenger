using Business.Exceptions;

namespace Business
{
    public class BussinessController
    {
        private IStore Store { get; set; }
        private Server Server { get; set; }

        public BussinessController(IStore store)
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

            return isValidPassword ? Server.ConnectClient(client) : "";
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
                throw new RecordNotFoundException("Client is not logged in");
            return loggedUser;
        }
    }
}