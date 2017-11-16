using System;
using Business;
using Business.Log;

namespace WcfServices
{
    public class LogRouter
    {
        private Logger logger;

        public LogRouter()
        {
            string serverIp = Utillities.GetLogServerIpFromConfigFile();
            logger = new Logger(serverIp);
        }

        public void LogAction(CrudAction action, Client client)
        {
            switch (action)
            {
                case CrudAction.Create:
                    var createClientEntry = new LogEntry
                    (
                        new CreateClientEntry()
                        {
                            NewClient = client,
                            Timestamp = DateTime.Now
                        }
                    );

                    logger.LogAction(createClientEntry);
                    break;

                case CrudAction.Update:
                    var updateClientEntry = new LogEntry
                    (
                        new UpdateClientEntry()
                        {
                            UpdatedClient = client,
                            Timestamp = DateTime.Now
                        }
                    );

                    logger.LogAction(updateClientEntry);
                    break;

                case CrudAction.Delete:
                    var deleteClientEntry = new LogEntry
                    (
                        new DeleteClientEntry()
                        {
                            DeletedClient = client,
                            Timestamp = DateTime.Now
                        }
                    );

                    logger.LogAction(deleteClientEntry);
                    break;
            }
        }
    }
}
