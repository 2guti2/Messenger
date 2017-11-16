using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class Logger
    {
        private string serverIp;

        public Logger(string serverIp)
        {
            this.serverIp = serverIp;
        }

        public void LogAction(LogEntry entry)
        {
            string queuePath = QueueUtillities.Path(serverIp);

            using (var messageQueue = new MessageQueue(queuePath))
            {
                var message = new System.Messaging.Message(entry);

                messageQueue.Send(message);
            }
        }
    }
}
