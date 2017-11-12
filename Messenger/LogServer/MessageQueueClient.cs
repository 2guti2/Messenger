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
            logEntries = new List<LogEntry>();
            this.businessController = businessController;
        }

        public void PrintLogs()
        {
            List<LogEntry> newEntries = businessController.GetLogEntries();

            if (newEntries != null)
            {
                foreach (LogEntry newEntry in newEntries)
                {
                    if (!logEntries.Contains(newEntry))
                    {
                        Console.WriteLine(newEntry);
                        logEntries.Add(newEntry);
                    }
                }
            }
        }
    }
}
