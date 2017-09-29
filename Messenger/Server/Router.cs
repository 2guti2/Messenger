using System;
using Protocol;

namespace Server
{
    public static class Router
    {
        private static readonly ServerController serverController = new ServerController();

        public static void Handle(Connection conn)
        {
            try
            {
                string[][][] message = conn.ReadMessage();
                var request = new Request(message);

                switch (request.Command)
                {
                    case Command.Login:
                        serverController.ConnectClient(conn, request);
                        break;
                    case Command.FriendshipRequest:
                        serverController.FriendshipRequest(conn, request);
                        break;
                    case Command.ListOfConnectedUsers:
                        serverController.ListConnectedUsers(conn, request);
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
                    case Command.Notifications:
                        serverController.ListNotifications(conn, request);
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
                    case Command.DisconnectUser:
                        serverController.DisconnectUser(conn, request);
                        break;
                    default:
                        serverController.InvalidCommand(conn);
                        break;
                }
            }
            catch (Exception e)
            {
                conn.SendMessage(new object[] {ResponseCode.InternalServerError, "There was a problem in the server: " + e.Message});
            }
            finally
            {
                conn.Close();
            }
        }
    }
}