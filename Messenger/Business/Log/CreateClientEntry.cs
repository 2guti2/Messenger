using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
