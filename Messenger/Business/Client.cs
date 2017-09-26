using System.Collections.Generic;
using Business.Exceptions;

namespace Business
{
    public class Client
    {
        public Client(string username, string password)
        {
            Username = username;
            Password = password;
            Friends = new List<Client>();
            FriendshipRequests = new List<FriendshipRequest>();
            Messages = new List<Message>();
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public List<Client> Friends { get; set; }
        public List<FriendshipRequest> FriendshipRequests { get; set; }
        public List<Message> Messages { get; set; }

        public override bool Equals(object obj)
        {
            Client toCompare = (Client) obj;
            return toCompare != null && Username.Equals(toCompare.Username);
        }

        public bool ValidatePassword(string clientPassword)
        {
            return Password.Equals(clientPassword);
        }

        public void AddFriendshipRequest(Client sender)
        {
            FriendshipRequests.Add(new FriendshipRequest(sender, this));
        }

        public FriendshipRequest ConfirmRequest(string requestId)
        {
            FriendshipRequest request = FriendshipRequests.Find(r => r.Id.ToString().Equals(requestId));
            if (request == null)
                throw new RecordNotFoundException("The request was not found");
            AddFriend(request.Sender);
            request.Sender.AddFriend(this);
            
            FriendshipRequests.Remove(request);

            return request;
        }

        private void AddFriend(Client client)
        {
            Friends.Add(client);
        }
    }
}