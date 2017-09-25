using System;
using System.Collections.Generic;
using Business;
using Business.Exceptions;
using Persistence;
using Protocol;

namespace Server
{
    public class ServerController
    {
        private readonly BusinessController businessController = new BusinessController(new Store());

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
                businessController.FriendshipRequest(loggedUser, username);
                conn.SendMessage(BuildResponse(ResponseCode.Ok, "Friendship request sent"));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Forbidden, e.Message));
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
        }

        public void InvalidCommand(Connection conn)
        {
            object[] response = BuildResponse(ResponseCode.BadRequest, "Unrecognizable command");
            conn.SendMessage(response);
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
    }
}