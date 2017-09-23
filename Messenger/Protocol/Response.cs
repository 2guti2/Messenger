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

        public string ResponseCode => responseObject[0][0][0];

        public bool HadSuccess()
        {
            return HasCode(Protocol.ResponseCode.Ok);
        }

        public string GetClientToken()
        {
            return responseObject[1][0][0];
        }

        public string ErrorMessage()
        {
            return responseObject[1][0][0];
        }

        private bool HasCode(ResponseCode responseCode)
        {
            return ResponseCode != null && responseCode.GetHashCode() == int.Parse(ResponseCode);
        }

        public List<string> UserList()
        {
            return new List<string>();
        }
    }
}