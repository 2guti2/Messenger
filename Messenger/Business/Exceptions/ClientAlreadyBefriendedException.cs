using System;

namespace Business.Exceptions
{
    public class ClientAlreadyBefriendedException : BusinessException
    {
        public ClientAlreadyBefriendedException() : base("The client is already your friend")
        {
        }
    }
}