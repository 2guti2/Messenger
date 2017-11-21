namespace Business.Log
{
    public class CreateClientEntry : LogEntryAttributes
    { 
        public Client NewClient { get; set; }

        public override string ToString()
        {
            return $"{Timestamp}: Client {NewClient?.Username} created.";
        }
    }
}
