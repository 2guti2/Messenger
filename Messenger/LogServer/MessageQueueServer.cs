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
        private BusinessController businessController;

        public MessageQueueServer(BusinessController businessController)
        {
            isServerRunning = true;
            QueuePath = QueueUtillities.QueueCreationPath();
            this.businessController = businessController;
        }

        public string QueuePath { get; }

        public void Start()
        {
            if (!MessageQueue.Exists(QueuePath)) MessageQueue.Create(QueuePath);

            var messageQueue = new MessageQueue(QueuePath)
            {
                Formatter = new XmlMessageFormatter(new[] { typeof(LogEntry) })
            };
            while (isServerRunning)
            {
                LogEntry entry = null;

                try
                {
                    System.Messaging.Message message = messageQueue.Receive();
                    entry = message?.Body as LogEntry;
                }
                catch (MessageQueueException) { }

                if (IsValidEntry(entry))
                    businessController.AddLogEntry(entry);
            }
        }

        public void Stop()
        {
            isServerRunning = false;
        }

        bool IsValidEntry(LogEntry entry)
        {
            return entry?.Text != null;
        }
    }
}
