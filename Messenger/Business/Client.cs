using System;
using System.Collections.Generic;
using System.Threading;
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
            Conversations = new List<Conversation>();
            Sessions = new List<Session>();
        }

        public string Username { get; private set; }
        public string Password { get; private set; }
        public List<Client> Friends { get; private set; }
        public List<FriendshipRequest> FriendshipRequests { get; private set; }
        public int FriendsCount => Friends.Count;
        public DateTime? ConnectedSince => Sessions.Find(session => session.Active)?.ConnectedSince;
        public int ConnectionsCount => Sessions.Count;
        private List<Session> Sessions { get; }
        private List<Conversation> Conversations { get; }

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
            if (HasFriend(sender) || sender.HasFriend(this)) throw new ClientAlreadyBefriendedException();
            if (sender.Equals(this)) throw new CantBefriendSelfException();
            if (HasSentFriendshitRequestFromClient(sender)) throw new RequestAlredySentException();

            if (sender.HasSentFriendshitRequestFromClient(this))
            {
                FriendshipRequest existingRequest = sender.RequestFromClient(this);
                sender.ConfirmRequest(existingRequest.Id.ToString());
            }
            else
            {
                FriendshipRequests.Add(new FriendshipRequest(sender, this));
            }
        }

        public void AddSession(Session session)
        {
            Sessions.Add(session);
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

        public void RejectRequest(string requestId)
        {
            FriendshipRequest request = FriendshipRequests.Find(r => r.Id.ToString().Equals(requestId));
            if (request == null)
                throw new RecordNotFoundException("The request was not found");

            FriendshipRequests.Remove(request);
        }

        private FriendshipRequest RequestFromClient(Client client)
        {
            return FriendshipRequests.Find(request => request.Sender.Equals(client));
        }

        private bool HasSentFriendshitRequestFromClient(Client client)
        {
            return FriendshipRequests.Exists(request => request.Sender.Equals(client));
        }

        private void AddFriend(Client client)
        {
            if (HasFriend(client) || client.HasFriend(this)) throw new ClientAlreadyBefriendedException();
            Friends.Add(client);
            client.Friends.Add(this);
        }

        public bool HasFriend(Client otherClient)
        {
            return Friends.Contains(otherClient);
        }
    }
}