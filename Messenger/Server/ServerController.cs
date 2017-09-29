﻿using System.Collections.Generic;
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
                if (unreadMessagesString.Count > 0)
                    conn.SendMessage(BuildResponse(ResponseCode.Ok, unreadMessagesString.ToArray()));
                else
                    conn.SendMessage(BuildResponse(ResponseCode.NotFound));
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

        public void ListNotifications(Connection conn, Request request)
        {
            try
            {
                Client loggedUser = CurrentClient(request);
                List<string> notifications = businessController.GetNotificationsOf(loggedUser);

                conn.SendMessage(BuildResponse(ResponseCode.Ok, notifications.ToArray()));
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
                var friendsUsernames = new List<string>();

                friends.ForEach(c => friendsUsernames.Add(c.Username));

                conn.SendMessage(BuildResponse(ResponseCode.Ok, friendsUsernames.ToArray()));
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
                var connectedUsernames = new List<string>();

                connectedUsers.ForEach(c => connectedUsernames.Add(c.Username));

                conn.SendMessage(BuildResponse(ResponseCode.Ok, connectedUsernames.ToArray()));
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

        private Client CurrentClient(Request req)
        {
            return businessController.GetLoggedClient(req.UserToken());
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

                allMessages.ForEach(ms => messagesString.Add(new[] { ms.Sender, ms.Content }));

                if (messagesString.Count > 0)
                    conn.SendMessage(BuildResponse(ResponseCode.Ok, messagesString.ToArray()));
                else
                    conn.SendMessage(BuildResponse(ResponseCode.NotFound));
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
    }
}