namespace Business.Log
{
    public class UploadFileEntry : LogEntryAttributes
    {
        public override string ToString()
        {
            return $"{Timestamp}: " + (ClientUsername ?? "Chat Server") + " uploaded a file.";
        }
    }
}
