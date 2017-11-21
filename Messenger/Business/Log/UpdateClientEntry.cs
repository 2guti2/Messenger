namespace Business.Log
{
    public class UpdateClientEntry : LogEntryAttributes
    {
        public Client UpdatedClient { get; set; }

        public override string ToString()
        {
            return $"{Timestamp}: Client {UpdatedClient?.Username} updated.";
        }
    }
}
