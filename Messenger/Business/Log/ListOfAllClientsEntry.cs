namespace Business.Log
{
    public class ListOfAllClientsEntry : LogEntryAttributes
    {
        public override string ToString()
        {
            return $"{Timestamp}: " + (ClientUsername ?? "Chat Server") + " listed all users.";
        }
    }
}
