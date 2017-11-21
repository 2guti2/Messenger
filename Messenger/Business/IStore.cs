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
        void DeleteClient(Client client);
        void ConnectClient(Client client, Session session);
        bool IsClientConnected(Client client);
        void DisconnectClient(string token);
        void UpdateClient(Client existingClient, Client newClient);
        Client FriendshipRequest(Client sender, string receiverUsername);
        FriendshipRequest ConfirmFriendshipRequest(Client currentClient, string requestId);
        FriendshipRequest RejectRequest(Client currentClient, string requestId);
        void AddLogEntry(LogEntry entryAttributes);
        List<LogEntry> GetLogEntries();
        LogEntry GetLastLogEntry();
    }
}
