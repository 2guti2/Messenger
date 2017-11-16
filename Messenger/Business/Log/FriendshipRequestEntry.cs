using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Log
{
    public class FriendshipRequestEntry : LogEntryAttributes
    {
        public string SenderUsername { get; set; }
        public string ReceiverUsername { get; set; }

        public override string ToString()
        {
            return $"{Timestamp}: {SenderUsername} sent a friendship request to {ReceiverUsername}";
        }
    }
}
