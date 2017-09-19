namespace Business
{
    public class FriendshipRequest
    {
        public FriendshipRequest()
        {

        }

        public Client Sender { get; set; }
        public Client Receiver { get; set; }
        public bool IsAccepted { get; set; }
    }
}
