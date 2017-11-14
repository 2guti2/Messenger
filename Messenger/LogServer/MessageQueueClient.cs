using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business;

namespace LogServer
{
    internal class MessageQueueClient
    {
        private MessageQueueServer msmqServer;
        private BusinessController businessController;
        private List<LogEntry> logEntries;

        public MessageQueueClient(MessageQueueServer msmqServer, BusinessController businessController)
        {
            this.msmqServer = msmqServer;
            msmqServer.NewEntry += new MessageQueueServer.NewEntryEventHandler(HandleNewEntry);
            logEntries = new List<LogEntry>();
            this.businessController = businessController;
        }

        public void HandleNewEntry()
        {
            Console.WriteLine(businessController.GetLastLogEntry());
        }
    }
}
