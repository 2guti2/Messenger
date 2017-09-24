using System;

namespace Business.Exceptions
{
    public class ClientNotConnectedException : Exception
    {
        public ClientNotConnectedException() : base("Client not connected")
        {
        }
    }
}