using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    #pragma warning disable CS0659
    public class Client
    {
        public Client(string username, string password)
        {
            Username = username;
            Password = password;
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
    }
}
