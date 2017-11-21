namespace Business.Log
{
    public class LogoutEntry : LogEntryAttributes
    {
        public override string ToString()
        {
            return $"{Timestamp}: {ClientUsername} logged out.";
        }
    }
}
