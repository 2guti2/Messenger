namespace Business.Log
{
    public class DownloadFileEntry : LogEntryAttributes
    {
        public override string ToString()
        {
            return $"{Timestamp}: " + (ClientUsername ?? "Chat Server") + " downloaded a file";
        }
    }
}
