using System.Collections.Generic;
using System.Linq;
using Business;
using Business.Exceptions;
using Protocol;
using System.Threading;

namespace Server
{
    public class ServerController
    {
        private readonly BusinessController businessController;

        public ServerController(BusinessController businessController)
        {
            this.businessController = businessController;
        }

        public void ConnectClient(Connection conn, Request req)
        {
            try
            {
                var client = new Client(req.Username(), req.Password());
                string token = businessController.Login(client);

                object[] response = string.IsNullOrEmpty(token)
                    ? BuildResponse(ResponseCode.NotFound, "Client not found")
                    : BuildResponse(ResponseCode.Ok, token);
                conn.SendMessage(response);
            }
            catch (ClientAlreadyConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Forbidden, e.Message));
            }
        }

        public void FriendshipRequest(Connection conn, Request req)
        {
            try
            {
                string username = req.Username();
                Client loggedUser = CurrentClient(req);
                Client receiver = businessController.FriendshipRequest(loggedUser, username);
                if (receiver.HasFriend(loggedUser))
                    conn.SendMessage(BuildResponse(ResponseCode.Ok, "Friend added"));
                else
                    conn.SendMessage(BuildResponse(ResponseCode.Created, "Friendship request sent"));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
            catch (BusinessException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Forbidden, e.Message));
            }
        }

        public void ReadMessage(Connection conn, Request request)
        {
            try
            {
                Client loggedUser = CurrentClient(request);
                List<Message> unreadMessages = businessController.UnreadMessages(loggedUser, request.Recipient());
                var unreadMessagesString = new List<string>();

                unreadMessages.ForEach(um => unreadMessagesString.Add(um.Content));
                conn.SendMessage(BuildResponse(ResponseCode.Ok, unreadMessagesString.ToArray()));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.NotFound, e.Message));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        public void ListMyFriends(Connection conn, Request request)
        {
            try
            {
                Client loggedUser = CurrentClient(request);
                List<Client> friends = businessController.GetFriendsOf(loggedUser);
                var clientFriends = new List<string[]>();

                friends.ForEach(c => clientFriends.Add(new[] {c.Username, c.FriendsCount.ToString()}));

                conn.SendMessage(BuildResponse(ResponseCode.Ok, clientFriends.ToArray()));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.NotFound, e.Message));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        public void GetFriendshipRequests(Connection conn, Request req)
        {
            try
            {
                Client currentClient = CurrentClient(req);
                string[][] requests = businessController.GetFriendshipRequests(currentClient);
                object[] response = BuildResponse(ResponseCode.Ok, requests);
                conn.SendMessage(response);
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        public void ConfirmFriendshipRequest(Connection conn, Request req)
        {
            try
            {
                Client currentClient = CurrentClient(req);
                string requestId = req.FriendshipRequestId();
                FriendshipRequest friendshipRequest =
                    businessController.ConfirmFriendshipRequest(currentClient, requestId);
                conn.SendMessage(BuildResponse(ResponseCode.Ok, friendshipRequest.Sender.Username));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.NotFound, e.Message));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
            catch (BusinessException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Forbidden, e.Message));
            }
        }

        public void RejectFriendshipRequest(Connection conn, Request req)
        {
            try
            {
                Client currentClient = CurrentClient(req);
                string requestId = req.FriendshipRequestId();
                businessController.RejectFriendshipRequest(currentClient, requestId);
                conn.SendMessage(BuildResponse(ResponseCode.Ok));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.NotFound, e.Message));
            }
        }

        public void ListConnectedUsers(Connection conn, Request request)
        {
            try
            {
                Client loggedUser = CurrentClient(request);
                List<Client> connectedUsers = businessController.GetLoggedClients();

                string[] connectedUsernames =
                    connectedUsers.Where(client => !client.Equals(loggedUser)).Select(c => c.Username)
                        .ToArray();

                conn.SendMessage(BuildResponse(ResponseCode.Ok, connectedUsernames));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.NotFound, e.Message));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        public void ListAllUsers(Connection conn, Request request)
        {
            try
            {
                Client loggedUser = CurrentClient(request);
                List<Client> clients = businessController.GetClients();

                string[] clientsUsernames =
                    clients.Where(client => !client.Equals(loggedUser)).Select(c => c.Username)
                        .ToArray();

                conn.SendMessage(BuildResponse(ResponseCode.Ok, clientsUsernames));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.NotFound, e.Message));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        public void DisconnectUser(Connection conn, Request request)
        {
            businessController.DisconnectClient(request.UserToken());
            conn.SendMessage(BuildResponse(ResponseCode.Ok, "Client disconnected"));
        }

        public void InvalidCommand(Connection conn)
        {
            object[] response = BuildResponse(ResponseCode.BadRequest, "Unrecognizable command");
            conn.SendMessage(response);
        }

        private object[] BuildResponse(ResponseCode responseCode, params object[] payload)
        {
            var responseList = new List<object>(payload);
            responseList.Insert(0, responseCode.GetHashCode());

            return responseList.ToArray();
        }

        public void SendMessage(Connection conn, Request request)
        {
            try
            {
                Client loggedUser = CurrentClient(request);
                string usernameFrom = loggedUser.Username;
                string usernameTo = request.Recipient();

                string message = request.Message;

                businessController.SendMessage(usernameFrom, usernameTo, message);
                conn.SendMessage(BuildResponse(ResponseCode.Ok));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.NotFound, e.Message));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        public void GetConversation(Connection conn, Request request)
        {
            try
            {
                Client loggedUser = CurrentClient(request);
                List<Message> allMessages = businessController.AllMessages(loggedUser, request.Recipient());

                var messagesString = new List<string[]>();

                allMessages.ForEach(ms => messagesString.Add(new[] {ms.Sender, ms.Content}));

                conn.SendMessage(BuildResponse(ResponseCode.Ok, messagesString.ToArray()));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.NotFound, e.Message));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        public void UplaodFile(Connection conn, Request req)
        {
            try
            {
                string fileName = req.FileName();
                FileUploader.UploadFile(conn, CurrentClient(req), fileName);
                conn.SendMessage(BuildResponse(ResponseCode.Ok, "File uploaded succesfully"));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        private Client CurrentClient(Request req)
        {
            return businessController.GetLoggedClient(req.UserToken());
        }
    }
}