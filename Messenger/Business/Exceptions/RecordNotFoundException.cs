using System;

namespace Business.Exceptions
{
    [Serializable]
    public class RecordNotFoundException : BusinessException
    {
        public RecordNotFoundException(string message) : base(message)
        {
        }
    }
}