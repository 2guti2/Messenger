namespace Business.Log
{
    public class ListOfConnectedUsersEntry : LogEntryAttributes
    {
        private string username = null;

        public ListOfConnectedUsersEntry(string username = null)
        {
            this.username = username;
        }

        public override string ToString()
        {
            return $"{Timestamp}: " + (username ?? "Chat Server") + " listed all connected users.";
        }
    }
}
