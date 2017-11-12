namespace Business.Exceptions
{
    public class RecordNotFoundException : BusinessException
    {
        public RecordNotFoundException(string message) : base(message)
        {
        }
    }
}