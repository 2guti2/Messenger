using System;

namespace Business.Exceptions
{
    [Serializable]
    public class ClientNotConnectedException : BusinessException
    {
        public ClientNotConnectedException() : base("Client not connected")
        {
        }
    }
}