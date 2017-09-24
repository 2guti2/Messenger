using System.Runtime.Remoting.Channels;

namespace Business
{
    public class FriendshipRequest
    {
        public FriendshipRequest(Client sender, Client receiver)
        {
            Sender = sender;
            Receiver = receiver;
            IsAccepted = false;
        }

        public Client Sender { get; set; }
        public Client Receiver { get; set; }
        public bool IsAccepted { get; set; }
    }
}