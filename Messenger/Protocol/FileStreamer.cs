namespace Protocol
{
    public class FileStreamer
    {
        public delegate void EventCalledDelegate();

        public event EventCalledDelegate OnProgressMade;
        
        public event EventCalledDelegate OnOperationCompleted;

        protected void ProgressMade()
        {
            OnProgressMade?.Invoke();
        }

        protected void OperationCompleted()
        {
            OnOperationCompleted?.Invoke();
        }
    }
}