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

        public void LogLogin(string username)
        {
            var loginEntry = new LogEntry
            (
                new LoginEntry()
                {
                    ClientUsername = username,
                    Timestamp = DateTime.Now
                }
            );

            logger.LogAction(loginEntry);
        }

        public void LogLogout(string username)
        {
            var logoutEntry = new LogEntry
            (
                new LogoutEntry()
                {
                    ClientUsername = username,
                    Timestamp = DateTime.Now
                }
            );

            logger.LogAction(logoutEntry);
        }

        internal void LogUploadFile(string uploadUsername)
        {
            var uploadFileEntry = new LogEntry
            (
                new UploadFileEntry()
                {
                    ClientUsername = uploadUsername,
                    Timestamp = DateTime.Now
                }
            );

            logger.LogAction(uploadFileEntry);
        }

        internal void LogDownloadFile(string downloadUsername)
        {
            var downloadFileEntry = new LogEntry
            (
                new DownloadFileEntry()
                {
                    ClientUsername = downloadUsername,
                    Timestamp = DateTime.Now
                }
            );

            logger.LogAction(downloadFileEntry);
        }
    }
}
