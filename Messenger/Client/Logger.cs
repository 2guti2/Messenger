﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business;
using System.Messaging;

namespace Client
{
    public class Logger
    {
        private string clientUsername;
        private string serverIp;

        public Logger(string clientUsername, string serverIp)
        {
            this.clientUsername = clientUsername;
            this.serverIp = serverIp;
        }

        public void LogAction(Command command)
        {
            //string queuePath = QueueUtillities.Path(serverIp);

            //if (!MessageQueue.Exists(queuePath)) MessageQueue.Create(queuePath);

            //var logEntry = new LogEntry()
            //{
            //    Action = command,
            //    ClientUsername = clientUsername,
            //    Timestamp = DateTime.Now
            //};

            //using (var messageQueue = new MessageQueue(queuePath))
            //{
            //    var message = new System.Messaging.Message(logEntry);

            //    messageQueue.Send(message);
            //}
        }
    }
}
