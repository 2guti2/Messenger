namespace Business
{
    public class LoginEntry : LogEntryAttributes
    {
        public override string ToString()
        {
            return $"{Timestamp}: {ClientUsername} logged in.";
        }
    }
}
