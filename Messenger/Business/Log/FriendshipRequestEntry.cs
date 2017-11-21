namespace Business.Log
{
    public class FriendshipRequestEntry : LogEntryAttributes
    {
        public string SenderUsername { get; set; }
        public string ReceiverUsername { get; set; }

        public override string ToString()
        {
            return $"{Timestamp}: {SenderUsername} sent a friendship request to {ReceiverUsername}";
        }
    }
}
