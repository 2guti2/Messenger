namespace Business.Log
{
    public class ListMyFriendsEntry : LogEntryAttributes
    {
        public override string ToString()
        {
            return $"{Timestamp}: {ClientUsername} listed his/her friends.";
        }
    }
}
