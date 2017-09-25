using System;

namespace Business.Exceptions
{
    public class ClientAlreadyConnectedException : Exception
    {
        public ClientAlreadyConnectedException() : base("Client already connected")
        {
        }
    }
}