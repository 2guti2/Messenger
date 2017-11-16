﻿using System;
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
                        serverController.FriendshipRequest(conn, request);
                        break;
                    case Command.ListOfConnectedUsers:
                        serverController.ListConnectedUsers(conn, request);
                        break;
                    case Command.ListOfAllClients:
                        serverController.ListAllUsers(conn, request);
                        break;
                    case Command.ListMyFriends:
                        serverController.ListMyFriends(conn, request);
                        break;
                    case Command.GetFriendshipRequests:
                        serverController.GetFriendshipRequests(conn, request);
                        break;
                    case Command.ConfirmFriendshipRequest:
                        serverController.ConfirmFriendshipRequest(conn, request);
                        break;
                    case Command.RejectFriendshipRequest:
                        serverController.RejectFriendshipRequest(conn, request);
                        break;
                    case Command.SendMessage:
                        serverController.SendMessage(conn, request);
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