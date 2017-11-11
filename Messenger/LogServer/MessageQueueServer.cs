using System;
using System.Messaging;
using Business;
using System.Collections;
using System.Collections.Generic;

namespace LogServer
{
    internal class MessageQueueServer
    {
        private bool isServerRunning;

        public MessageQueueServer(string serverIp)
        {
            isServerRunning = true;
            QueuePath = QueueUtillities.Path(serverIp);
            LogEntries = new List<LogEntry>();
        }

        public string QueuePath { get; }

        //this needs to be part of the remote store
        public List<LogEntry> LogEntries { get; }

        public void Start()
        {
            var messageQueue = new MessageQueue(QueuePath)
            {
                Formatter = new XmlMessageFormatter(new Type[] { typeof(LogEntry) })
            };

            while (isServerRunning)
            {
                LogEntry entry = null;

                try
                {
                    System.Messaging.Message message = messageQueue.Receive();
                    entry = message.Body as LogEntry;
                }
                catch (MessageQueueException) { }

                if(IsValidEntry(entry))
                    LogEntries.Add(entry);
            }
        }

        public void Stop()
        {
            isServerRunning = false;
        }

        bool IsValidEntry(LogEntry entry)
        {
            return entry != null && entry.ClientUsername != null;
        }
    }
}
