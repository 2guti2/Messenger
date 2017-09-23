using System;
using System.Security.Cryptography;
using System.Security.Principal;
using Business;
using Protocol;

namespace Server
{
    public class ServerController
    {
        public void ConnectClient(Connection conn, Request req)
        {
            var token = GenerateRandomToken();
            Console.WriteLine("Sent token: " + token);
            object[] response = {ResponseCode.Ok.GetHashCode(), token};
            conn.SendMessage(response);
        }

        public void InvalidCommand(Connection conn, Request req)
        {
            object[] response = {ResponseCode.BadRequest.GetHashCode(), "Unrecognizable command"};
            conn.SendMessage(response);
        }
        
        public static string GenerateRandomToken(int length = 8)
        {
            RNGCryptoServiceProvider cryptRNG = new RNGCryptoServiceProvider();
            byte[] tokenBuffer = new byte[length];
            cryptRNG.GetBytes(tokenBuffer);
            return Convert.ToBase64String(tokenBuffer);
        }
    }
}
