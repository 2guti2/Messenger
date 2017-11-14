using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business;
using Business.Log;

namespace Server
{
    public class LogRouter
    {
        private Logger logger;
        public LogRouter()
        {
            string logServerIp = Utillities.GetLogServerIpFromConfigFile();
            logger = new Logger(logServerIp);
        }

        public void LogAction(Command command, string username)
        {
            switch (command)
            {
                case Command.Login:
                    var loginEntry = new LogEntry
                    (
                        new LoginEntry()
                        {
                            ClientUsername = username,
                            Timestamp = DateTime.Now
                        }
                    );

                    logger.LogAction(loginEntry);
                    break;

                case Command.FriendshipRequest:

                    break;

                case Command.ListOfConnectedUsers:

                    break;
                case Command.ListOfAllClients:

                    break;
                case Command.ListMyFriends:

                    break;
                case Command.GetFriendshipRequests:

                    break;
                case Command.ConfirmFriendshipRequest:

                    break;
                case Command.RejectFriendshipRequest:

                    break;
                case Command.SendMessage:

                    break;
                case Command.ReadMessage:

                    break;
                case Command.GetConversation:

                    break;
                case Command.UploadFile:

                    break;
                case Command.ListClientFiles:

                    break;
                case Command.DownloadFile:

                    break;
                case Command.DisconnectUser:
                    
                    var logoutEntry = new LogEntry
                    (
                        new LogoutEntry()
                        {
                            ClientUsername = username,
                            Timestamp = DateTime.Now
                        }
                    );

                    logger.LogAction(logoutEntry);
                    break;
            }
        }
    }
}
