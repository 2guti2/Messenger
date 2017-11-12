namespace Business.Exceptions
{
    public class CantBefriendSelfException : BusinessException
    {
        public CantBefriendSelfException() : base("You can't befriend yourself")
        {
        }
    }
}