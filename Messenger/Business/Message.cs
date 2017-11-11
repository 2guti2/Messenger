using System;

namespace Business
{
    [Serializable]
    public class Message
    {
        public Message()
        {

        }

        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool Read { get; set; }
    }
}