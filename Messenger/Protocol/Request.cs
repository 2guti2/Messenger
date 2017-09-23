using Business;

namespace Protocol
{
    public class Request
    {
        private readonly string[][][] requestObject;
        
        public Request(string[][][] request)
        {
            requestObject = request;
        }

        public Command Command => (Command) int.Parse(requestObject[0][0][0]);
    }
}