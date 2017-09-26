using System;

namespace Business.Exceptions
{
    public class RequestAlredySentException : BusinessException
    {
        public RequestAlredySentException() : base("Friendship request already sent")
        {
        }
    }
}