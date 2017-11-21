using System;

namespace Business.Exceptions
{
    [Serializable]
    public class RequestAlredySentException : BusinessException
    {

        public RequestAlredySentException() : base("Friendship request already sent")
        {
        }
    }
}