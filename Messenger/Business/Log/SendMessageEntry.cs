namespace Business.Log
{
    public class SendMessageEntry : LogEntryAttributes
    {
        public string SenderUsername { get; set; }
        public string RecipientUsername { get; set; }

        public override string ToString()
        {
            return $"{Timestamp}: {SenderUsername} sent a message to {RecipientUsername}.";
        }
    }
}
