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

        public bool HadSuccess() => HasCode(Protocol.ResponseCode.Ok) || HasCode(Protocol.ResponseCode.Created);

        public string GetClientToken() => responseObject[1][0][0];

        public string GetUsername() => responseObject[1][0][0];

        public string ErrorMessage() => responseObject[1][0][0];

        public string ServerMessage() => responseObject[1][0][0];

        public List<string> UserList()
        {
            var users = new List<string>();
            for (var i = 1; i < responseObject.Length; i++)
                users.Add(responseObject[i][0][0]);
            return users;
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

        private string ResponseCode => responseObject[0][0][0];

        private bool HasCode(ResponseCode responseCode)
        {
            return ResponseCode != null && responseCode.GetHashCode() == int.Parse(ResponseCode);
        }
    }
}