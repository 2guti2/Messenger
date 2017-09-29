using Business;
using System.Collections.Generic;
using System.Collections.Generic;
using Business;

namespace Protocol
{
    public class Response
    {
        private readonly string[][][] responseObject;

        public Response(string[][][] response)
        {
            responseObject = response;
        }

        public string ResponseCode => responseObject[0][0][0];

        public string Message => responseObject[1][0][0];

        public List<string> Messages()
        {
            var messages = new List<string>();

            for (int i = 1; i < responseObject.Length; i++)
                messages.Add(responseObject[i][0][0]);

            return messages;
        }

        public bool HadSuccess()
        {
            return HasCode(Protocol.ResponseCode.Ok);
        }

        public string GetClientToken()
        {
            return responseObject[1][0][0];
        }

        public string GetUsername()
        {
            return responseObject[1][0][0];
        }

        public string ErrorMessage()
        {
            return responseObject[1][0][0];
        }

        public List<string> UserList()
        {
            var users = new List<string>();
            for (int i = 1; i < responseObject.Length; i++)
                users.Add(responseObject[i][0][0]);
            return users;
        }

        public List<int> Notifications()
        {
            var notifications = new List<int>();
            for (int i = 1; i < responseObject.Length; i++)
                notifications.Add(int.Parse(responseObject[i][0][0]));
            return notifications;
        }

        public string[][] FriendshipRequests()
        {
            var receivedRequests = new string[responseObject.Length - 1][];
            for (var i = 1; i < responseObject.Length; i++)
            {
                string guid = responseObject[i][0][0];
                string clientName = responseObject[i][1][0];
                receivedRequests[i - 1] = new[] {guid, clientName};
            }

            return receivedRequests;
        }

        private bool HasCode(ResponseCode responseCode)
        {
            return ResponseCode != null && responseCode.GetHashCode() == int.Parse(ResponseCode);
        }
    }
}