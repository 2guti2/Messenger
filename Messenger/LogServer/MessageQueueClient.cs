using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogServer
{
    internal class MessageQueueClient
    {
        private MessageQueueServer msmqServer;

        public MessageQueueClient(MessageQueueServer msmqServer)
        {
            this.msmqServer = msmqServer;
        }

        public void Menu()
        {
            
        }
    }
}
