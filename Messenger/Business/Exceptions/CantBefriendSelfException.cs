using System;

namespace Business.Exceptions
{
    [Serializable]
    public class CantBefriendSelfException : BusinessException
    {
        public CantBefriendSelfException() : base("You can't befriend yourself")
        {
        }
    }
}