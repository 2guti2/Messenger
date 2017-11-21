using System;
using Business;
using Business.Log;

namespace ChatServer
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

        internal void LogUploadFile(string uploadUsername = null)
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

        internal void LogFriendshipRequest(string friendUsernameSendingRequest, string friendUsernameReceivingRequest)
        {
            var friendshipRequestEntry = new LogEntry
            (
                new FriendshipRequestEntry()
                {
                    SenderUsername = friendUsernameSendingRequest,
                    ReceiverUsername = friendUsernameReceivingRequest,
                    Timestamp = DateTime.Now
                }
            );

            logger.LogAction(friendshipRequestEntry);
        }

        internal void LogListOfAllClients(string allClientsClientUsername = null)
        {
            var listOfAllClientsEntry = new LogEntry
            (
                new ListOfAllClientsEntry()
                {
                    ClientUsername = allClientsClientUsername,
                    Timestamp = DateTime.Now
                }
            );

            logger.LogAction(listOfAllClientsEntry);
        }

        internal void LogListMyFriends(string myFriendsClientUsername)
        {
            var listMyFriendsEntry = new LogEntry
            (
                new ListMyFriendsEntry()
                {
                    ClientUsername = myFriendsClientUsername,
                    Timestamp = DateTime.Now
                }
            );

            logger.LogAction(listMyFriendsEntry);
        }

        internal void LogConfirmationOfFriendshipRequest(FriendshipRequest fr)
        {
            var confirmationOfFR = new LogEntry
            (
                new ConfirmFriendshipRequestEntry()
                {
                    ConfirmerUsername = fr.Receiver?.Username,
                    SenderUsername = fr.Sender?.Username,
                    Timestamp = DateTime.Now
                }
            );

            logger.LogAction(confirmationOfFR);
        }

        internal void LogRejectionOfFriendshipRequest(FriendshipRequest friendshipRequest)
        {
            var rejectionOfFR = new LogEntry
            (
                new RejectFriendshipRequestEntry()
                {
                    ConfirmerUsername = friendshipRequest.Receiver?.Username,
                    SenderUsername = friendshipRequest.Sender?.Username,
                    Timestamp = DateTime.Now
                }
            );

            logger.LogAction(rejectionOfFR);
        }

        internal void LogListOfConnectedUsers(string clientUsername = null)
        {
            var listOfConnectedClientsEntry = new LogEntry
            (
                new ListOfConnectedUsersEntry(clientUsername)
                {
                    Timestamp = DateTime.Now
                }
            );

            logger.LogAction(listOfConnectedClientsEntry);
        }

        internal void LogSendMessage(string senderUsername, string recipientUsername)
        {
            var sendMessageEntry = new LogEntry
            (
                new SendMessageEntry()
                {
                    SenderUsername = senderUsername,
                    RecipientUsername = recipientUsername,
                    Timestamp = DateTime.Now
                }
            );

            logger.LogAction(sendMessageEntry);
        }

        internal void LogDownloadFile(string downloadUsername = null)
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
