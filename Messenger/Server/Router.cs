using System;
using Business;
using Protocol;

namespace Server
{
    public class Router
    {
        private readonly ServerController serverController;
        private LogRouter logRouter;

        public Router(ServerController serverController)
        {
            this.serverController = serverController;
            logRouter = new LogRouter();
        }

        public void Handle(Connection conn)
        {
            try
            {
                string[][][] message = conn.ReadMessage();
                var request = new Request(message);

                switch (request.Command)
                {
                    case Command.Login:
                        serverController.ConnectClient(conn, request);
                        logRouter.LogLogin(request.Username());
                        break;
                    case Command.FriendshipRequest:
                        string friendUsernameSendingRequest = GetClientUsernameFromRequest(request);
                        string friendUsernameReceivingRequest = request.Username();
                        serverController.FriendshipRequest(conn, request);
                        logRouter.LogFriendshipRequest(friendUsernameSendingRequest, friendUsernameReceivingRequest);
                        break;
                    case Command.ListOfConnectedUsers:
                        string connectedUsersClientUsername = GetClientUsernameFromRequest(request);
                        serverController.ListConnectedUsers(conn, request);
                        logRouter.LogListOfConnectedUsers(connectedUsersClientUsername);
                        break;
                    case Command.ListOfAllClients:
                        string allClientsClientUsername = GetClientUsernameFromRequest(request);
                        serverController.ListAllUsers(conn, request);
                        logRouter.LogListOfAllClients(allClientsClientUsername);
                        break;
                    case Command.ListMyFriends:
                        string myFriendsClientUsername = GetClientUsernameFromRequest(request);
                        serverController.ListMyFriends(conn, request);
                        logRouter.LogListMyFriends(myFriendsClientUsername);
                        break;
                    case Command.GetFriendshipRequests:
                        serverController.GetFriendshipRequests(conn, request);
                        break;
                    case Command.ConfirmFriendshipRequest:
                        FriendshipRequest fr = serverController.ConfirmFriendshipRequest(conn, request);
                        logRouter.LogConfirmationOfFriendshipRequest(fr);
                        break;
                    case Command.RejectFriendshipRequest:
                        FriendshipRequest friendshipRequest = serverController.RejectFriendshipRequest(conn, request);
                        logRouter.LogRejectionOfFriendshipRequest(friendshipRequest);
                        break;
                    case Command.SendMessage:
                        string senderUsername = GetClientUsernameFromRequest(request);
                        string recipientUsername = request.Recipient();
                        serverController.SendMessage(conn, request);
                        logRouter.LogSendMessage(senderUsername, recipientUsername);
                        break;
                    case Command.ReadMessage:
                        serverController.ReadMessage(conn, request);
                        break;
                    case Command.GetConversation:
                        serverController.GetConversation(conn, request);
                        break;                    
                    case Command.UploadFile:
                        string uploadUsername = GetClientUsernameFromRequest(request);
                        serverController.UploadFile(conn, request);
                        logRouter.LogUploadFile(uploadUsername);
                        break;
                    case Command.ListClientFiles:
                        serverController.ListClientFiles(conn, request);
                        break;
                    case Command.DownloadFile:
                        string downloadUsername = GetClientUsernameFromRequest(request);
                        serverController.DownloadFile(conn, request);
                        logRouter.LogDownloadFile(downloadUsername);
                        break;
                    case Command.DisconnectUser:
                        string logoutUsername = GetClientUsernameFromRequest(request);
                        serverController.DisconnectUser(conn, request);
                        logRouter.LogLogout(logoutUsername);
                        break;
                    default:
                        serverController.InvalidCommand(conn);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception thrown: " + e.Message);
                Console.WriteLine(e.StackTrace);
                conn.SendMessage(new object[] {ResponseCode.InternalServerError.GetHashCode(), "There was a problem in the server"});
            }
            finally
            {
                try
                {
                    conn.Close();
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to close connection.");
                }
            }
        }

        private string GetClientUsernameFromRequest(Request req)
        {
            return serverController.BusinessController.GetLoggedClient(req.UserToken())?.Username;
        }
    }
}