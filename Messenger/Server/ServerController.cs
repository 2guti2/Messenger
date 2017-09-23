﻿using System.Collections.Generic;
using Business;
using Business.Exceptions;
using Persistence;
using Protocol;
using Persistence;
using System.Collections.Generic;

namespace Server
{
    public class ServerController
    {
        private readonly BussinessController BussinessController = new BussinessController(new Store());

        public void ConnectClient(Connection conn, Request req)
        {
            var client = new Client(req.Username(), req.Password());
            string token = BussinessController.Login(client);

            object[] response;
            if (string.IsNullOrEmpty(token))
                response = BuildResponse(ResponseCode.NotFound, "Client not found");
            else
                response = BuildResponse(ResponseCode.Ok, token);
            conn.SendMessage(response);
        }

        public void FriendshipRequest(Connection conn, Request req)
        {
            try
            {
                string username = req.Username();
                Client loggedUser = CurrentClient(req);
                BussinessController.FriendshipRequest(loggedUser, username);
                conn.SendMessage(BuildResponse(ResponseCode.Ok, "Friendship request sent"));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.NotFound, e.Message));
            }
        }

        public void InvalidCommand(Connection conn, Request req)
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
            return BussinessController.GetLoggedClient(req.UserToken());
        }

        internal void ListConnectedUsers(Connection conn, Request request)
        {
            if (IsTokenCorrect(request.Token))
            {
                object[] response = { ResponseCode.Ok.GetHashCode(), new string[] { "uno", "dos" } };
                conn.SendMessage(response);
            }
        }

        private bool IsTokenCorrect(string token)
        {
            return true;
        }
    }
}