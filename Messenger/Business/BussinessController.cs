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
            {
                Store.AddClient(client);
            }
            var storedClient = Store.GetClient(client.Username);
            bool isValidPassword = storedClient.ValidatePassword(client.Password);

            return isValidPassword ? Server.ConnectClient(client) : "";
        }
    }
}
