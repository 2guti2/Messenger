using System.Collections.Generic;
using Business;
using Persistence;
using Protocol;

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
    }
}