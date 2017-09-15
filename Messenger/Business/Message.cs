using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class Message
    {
        public Message()
        {

        }

        public Client Sender { get; set; }
        public Client Receiver { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
