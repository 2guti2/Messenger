namespace Business.Log
{
    public class DeleteClientEntry : LogEntryAttributes
    {
        public Client DeletedClient { get; set; }

        public override string ToString()
        {
            return $"{Timestamp}: Client {DeletedClient?.Username} deleted.";
        }
    }
}
