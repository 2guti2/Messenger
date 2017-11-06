using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using Business;

namespace Server
{
    public class MessageQueueServer
    {
        public MessageQueueServer(string serverIp)
        {
            QueuePath = QueueUtillities.Path(serverIp);
        }

        public string QueuePath { get; }

        public void Start()
        {
            var messageQueue = new MessageQueue(QueuePath)
            {
                Formatter = new XmlMessageFormatter(new Type[] { typeof(LogEntry) })
            };

            while (true)
            {
                LogEntry entry = null;

                try
                {
                    System.Messaging.Message message = messageQueue.Receive();
                    entry = message.Body as LogEntry;
                }
                catch (MessageQueueException) { }

                if(IsValidEntry(entry))
                    QueueUtillities.SaveEntry(entry);
            }
        }

        bool IsValidEntry(LogEntry entry)
        {
            return entry != null && entry.ClientUsername != null;
        }
    }
}
