using System.Collections.Generic;

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
            Conversations = new List<Conversation>();
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public List<Client> Friends { get; set; }
        public List<FriendshipRequest> FriendshipRequests { get; set; }
        public List<Conversation> Conversations { get; set; }

        public override bool Equals(object obj)
        {
            var toCompare = (Client) obj;
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
    }
}