using System;

namespace Business.Exceptions
{
    [Serializable]
    public class ClientAlreadyConnectedException : BusinessException
    {
        public ClientAlreadyConnectedException() : base("Client already connected")
        {
        }
    }
}