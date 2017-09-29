using System.Collections.Generic;

namespace Business
{
    public interface IStore
    {
        bool ClientExists(Client client);
        void AddClient(Client client);
        Client GetClient(string clientUsername);
        List<Client> GetFriendsOf(Client client);
        void SendMessage(string usernameFrom, string usernameTo, string message);
        List<Message> UnreadMessages(Client of, string from);
        List<Message> AllMessages(Client of, string @from);
        List<Client> GetClients();
    }
}
