using System;

namespace Business.Exceptions
{
    [Serializable]
    public class ClientAlreadyExistsException : BusinessException
    {
        public ClientAlreadyExistsException() : base("Client already exists") { }
    }
}
