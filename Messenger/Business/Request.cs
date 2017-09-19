namespace Business
{
    public class Request
    {
        public Request() { }

        public Request(ActionType actionType, string body)
        {
            ActionType = actionType;
            Body = body;
        }

        public ActionType ActionType { get; set; }
        public string Body { get; set; }
    }
}
