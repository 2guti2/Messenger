using System;
using System.Runtime.Remoting.Channels;

namespace Business
{
    public class FriendshipRequest
    {
        public FriendshipRequest(Client sender, Client receiver)
        {
            Sender = sender;
            Receiver = receiver;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
        public Client Sender { get; set; }
        public Client Receiver { get; set; }

        public override bool Equals(object obj)
        {
            var toCompare = obj as FriendshipRequest;
            return toCompare != null && Id.Equals(toCompare.Id);
        }
    }
}