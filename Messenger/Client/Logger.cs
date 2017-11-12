using System;
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

            //var logEntry = new AddLogEntry()
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
