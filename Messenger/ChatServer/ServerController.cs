using System.Collections.Generic;
using System.Linq;
using Business;
using Business.Exceptions;
using Protocol;

namespace ChatServer
{
    public class ServerController
    {
        private const string UploadFolder = "uploads";
        public readonly BusinessController BusinessController;

        public ServerController(BusinessController businessController)
        {
            BusinessController = businessController;
        }

        public void ConnectClient(Connection conn, Request req)
        {
            try
            {
                var client = new Client(req.Username(), req.Password());
                string token = BusinessController.Login(client);

                object[] response = string.IsNullOrEmpty(token)
                    ? BuildResponse(ResponseCode.NotFound, "Client not found")
                    : BuildResponse(ResponseCode.Ok, token);
                conn.SendMessage(response);
            }
            catch (ClientAlreadyConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Forbidden, e.Message));
            }
        }

        public void FriendshipRequest(Connection conn, Request req)
        {
            try
            {
                string username = req.Username();
                Client loggedUser = CurrentClient(req);
                Client receiver = BusinessController.FriendshipRequest(loggedUser, username);
                if (receiver.HasFriend(loggedUser))
                    conn.SendMessage(BuildResponse(ResponseCode.Ok, "Friend added"));
                else
                    conn.SendMessage(BuildResponse(ResponseCode.Created, "Friendship request sent"));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
            catch (BusinessException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Forbidden, e.Message));
            }
        }

        public void ReadMessage(Connection conn, Request request)
        {
            try
            {
                Client loggedUser = CurrentClient(request);
                List<Message> unreadMessages = BusinessController.UnreadMessages(loggedUser, request.Recipient());
                var unreadMessagesString = new List<string>();

                unreadMessages.ForEach(um => unreadMessagesString.Add(um.Content));
                conn.SendMessage(BuildResponse(ResponseCode.Ok, unreadMessagesString.ToArray()));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.NotFound, e.Message));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        public void ListMyFriends(Connection conn, Request request)
        {
            try
            {
                Client loggedUser = CurrentClient(request);
                List<Client> friends = BusinessController.GetFriendsOf(loggedUser);
                var clientFriends = new List<string[]>();

                friends.ForEach(c => clientFriends.Add(new[] {c.Username, c.FriendsCount.ToString()}));

                conn.SendMessage(BuildResponse(ResponseCode.Ok, clientFriends.ToArray()));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.NotFound, e.Message));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        public void GetFriendshipRequests(Connection conn, Request req)
        {
            try
            {
                Client currentClient = CurrentClient(req);
                string[][] requests = BusinessController.GetFriendshipRequests(currentClient);
                object[] response = BuildResponse(ResponseCode.Ok, requests);
                conn.SendMessage(response);
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        public FriendshipRequest ConfirmFriendshipRequest(Connection conn, Request req)
        {
            FriendshipRequest friendshipRequest = null;
            try
            {
                Client currentClient = CurrentClient(req);
                string requestId = req.FriendshipRequestId();
                friendshipRequest =
                    BusinessController.ConfirmFriendshipRequest(currentClient, requestId);
                conn.SendMessage(BuildResponse(ResponseCode.Ok, friendshipRequest.Sender.Username));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.NotFound, e.Message));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
            catch (BusinessException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Forbidden, e.Message));
            }

            return friendshipRequest;
        }

        public FriendshipRequest RejectFriendshipRequest(Connection conn, Request req)
        {
            FriendshipRequest fr = null;
            try
            {
                Client currentClient = CurrentClient(req);
                string requestId = req.FriendshipRequestId();
                fr = BusinessController.RejectFriendshipRequest(currentClient, requestId);
                conn.SendMessage(BuildResponse(ResponseCode.Ok));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.NotFound, e.Message));
            }
            return fr;
        }

        public void ListConnectedUsers(Connection conn, Request request)
        {
            try
            {
                Client loggedUser = CurrentClient(request);
                List<Client> connectedUsers = BusinessController.GetLoggedClients();

                string[] connectedUsernames =
                    connectedUsers.Where(client => !client.Equals(loggedUser)).Select(c => c.Username)
                        .ToArray();

                conn.SendMessage(BuildResponse(ResponseCode.Ok, connectedUsernames));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.NotFound, e.Message));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        public void ListAllUsers(Connection conn, Request request)
        {
            try
            {
                Client loggedUser = CurrentClient(request);
                List<Client> clients = BusinessController.GetClients();

                string[] clientsUsernames =
                    clients.Where(client => !client.Equals(loggedUser)).Select(c => c.Username)
                        .ToArray();

                conn.SendMessage(BuildResponse(ResponseCode.Ok, clientsUsernames));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.NotFound, e.Message));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        public void DisconnectUser(Connection conn, Request request)
        {
            BusinessController.DisconnectClient(request.UserToken());
            conn.SendMessage(BuildResponse(ResponseCode.Ok, "Client disconnected"));
        }

        public void InvalidCommand(Connection conn)
        {
            object[] response = BuildResponse(ResponseCode.BadRequest, "Unrecognizable command");
            conn.SendMessage(response);
        }

        public void SendMessage(Connection conn, Request request)
        {
            try
            {
                Client loggedUser = CurrentClient(request);
                string usernameFrom = loggedUser.Username;
                string usernameTo = request.Recipient();

                string message = request.Message;

                BusinessController.SendMessage(usernameFrom, usernameTo, message);
                conn.SendMessage(BuildResponse(ResponseCode.Ok));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.NotFound, e.Message));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        public void GetConversation(Connection conn, Request request)
        {
            try
            {
                Client loggedUser = CurrentClient(request);
                List<Message> allMessages = BusinessController.AllMessages(loggedUser, request.Recipient());

                var messagesString = new List<string[]>();

                allMessages.ForEach(ms => messagesString.Add(new[] {ms.Sender, ms.Content}));

                conn.SendMessage(BuildResponse(ResponseCode.Ok, messagesString.ToArray()));
            }
            catch (RecordNotFoundException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.NotFound, e.Message));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        public void UploadFile(Connection conn, Request req)
        {
            try
            {
                AuthenticateClient(req);
                string fileName = req.FileName();
                conn.SendMessage(BuildResponse(ResponseCode.Ok));
                string clientDirectory = UploadsDirectory();
                var downloader = new FileDownloader(clientDirectory, fileName);
                downloader.DownloadFile(conn);
                conn.SendMessage(BuildResponse(ResponseCode.Ok, "File uploaded succesfully"));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        public void ListClientFiles(Connection conn, Request req)
        {
            try
            {
                AuthenticateClient(req);
                List<string> files = FileLister.ListFiles(UploadsDirectory());
                conn.SendMessage(BuildResponse(ResponseCode.Ok, files.ToArray()));
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        public void DownloadFile(Connection conn, Request req)
        {
            try
            {
                AuthenticateClient(req);
                string uploadsDirectory = UploadsDirectory();
                List<string> files = FileLister.ListFiles(uploadsDirectory);
                string selectedFile = files[req.SelectedFileIndex()];
                string fullFilePath = $@"{uploadsDirectory}\{selectedFile}";
                var uploader = new FileUploader(fullFilePath);
                conn.SendMessage(BuildResponse(ResponseCode.Ok, uploader.ExpectedTicks));
                uploader.UploadFile(conn);
            }
            catch (ClientNotConnectedException e)
            {
                conn.SendMessage(BuildResponse(ResponseCode.Unauthorized, e.Message));
            }
        }

        public Client CurrentClient(Request req)
        {
            return BusinessController.GetLoggedClient(req.UserToken());
        }

        private void AuthenticateClient(Request req)
        {
            CurrentClient(req);
        }

        private object[] BuildResponse(ResponseCode responseCode, params object[] payload)
        {
            var responseList = new List<object>(payload);
            responseList.Insert(0, responseCode.GetHashCode());

            return responseList.ToArray();
        }

        private string UploadsDirectory()
        {
            return $@"{UploadFolder}\";
        }
    }
}