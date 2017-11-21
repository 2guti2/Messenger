namespace Business.Log
{
    public class RejectFriendshipRequestEntry : LogEntryAttributes
    {
        public string SenderUsername { get; set; }
        public string ConfirmerUsername { get; set; }

        public override string ToString()
        {
            return $"{Timestamp}: {ConfirmerUsername} rejected a friendship request from {SenderUsername}.";
        }
    }
}
