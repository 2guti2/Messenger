using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Log
{
    public class SendMessageEntry : LogEntryAttributes
    {
        public string SenderUsername { get; set; }
        public string RecipientUsername { get; set; }

        public override string ToString()
        {
            return $"{Timestamp}: {SenderUsername} sent a message to {RecipientUsername}.";
        }
    }
}
