using Business;
using System.Collections.Generic;

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

        public string Message => responseObject[1][0][0];

        public List<string> Messages()
        {
            var messages = new List<string>();

            for (int i = 1; i < responseObject.Length; i++)
                messages.Add(responseObject[i][0][0]);

            return messages;
        }

        public List<string> Conversation(string me)
        {
            var conversation = new List<string>();

            for (int i = 1; i < responseObject.Length; i++)
            {
                string sender = responseObject[i][0][0];
                string message = (sender.Equals(me) ? "" : sender + ": ") + responseObject[i][1][0];
                conversation.Add(message);
            }

            return conversation;
        }

        public string GetClientToken() => responseObject[1][0][0];

        public string GetUsername() => responseObject[1][0][0];

        public string ErrorMessage() => responseObject[1][0][0];

        public string ServerMessage() => responseObject[1][0][0];


        public int FileChunks() => int.Parse(responseObject[1][0][0]);

        public List<string> UserList()
        {
            var users = new List<string>();
            for (var i = 1; i < responseObject.Length; i++)
                users.Add(responseObject[i][0][0]);
            return users;
        }

        public List<string> UserWithFriendsCountList()
        {
            var users = new List<string>();
            for (var i = 1; i < responseObject.Length; i++)
                users.Add($"- {responseObject[i][0][0]} ({responseObject[i][1][0]})");
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
        
        public List<string> FilesList()
        {
            var list = new List<string>();
            for (var i = 1; i < responseObject.Length; i++)
                list.Add(responseObject[i][0][0]);

            return list;
        }

        private string ResponseCode => responseObject[0][0][0];

        private bool HasCode(ResponseCode responseCode)
        {
            return ResponseCode != null && responseCode.GetHashCode() == int.Parse(ResponseCode);
        }
    }
}