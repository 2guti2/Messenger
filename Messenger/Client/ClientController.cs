using System;
using System.Collections.Generic;
using Protocol;
using UI;
using System.Configuration;
using System.Net.Sockets;
using Business;
using System.Threading;
using System.Messaging;

namespace Client
{
    public class ClientController
    {
        private const double WaitTimeAumentation = 1.5;
        private const int InitialWaitTime = 100;
        private readonly ClientProtocol clientProtocol;
        private string clientToken;
        private string clientUsername;
        private string serverIp;
        private Logger logger;

        public ClientController()
        {
            clientToken = "";
            clientUsername = null;
            serverIp = GetServerIpFromConfigFile();
            int serverPort = GetServerPortFromConfigFile();
            string clientIp = GetClientIpFromConfigFile();
            int clientPort = GetClientPortFromConfigFile();
            clientProtocol = new ClientProtocol(serverIp, serverPort, clientIp, clientPort);
        }

        public void DisconnectFromServer()
        {
            Connection connection = clientProtocol.ConnectToServer();
            connection.SendMessage(BuildRequest(Command.DisconnectUser));
            var response = new Response(connection.ReadMessage());
            Console.WriteLine(response.HadSuccess() ? "Disconnected" : response.ErrorMessage());
            logger.LogAction(Command.DisconnectUser);
        }

        internal void LoopMenu()
        {
            Init();
            while (true)
            {
                Console.WriteLine(ClientUI.Title(clientUsername));
                int option = Menus.MapInputWithMenuItemsList(MenuOptions());
                MapOptionToActionOfMainMenu(option);
                ClientUI.Clear();
            }
        }

        private void Init()
        {
            Console.WriteLine(ClientUI.Title());
            ConnectToServer();
            ClientUI.Clear();
        }

        private bool ListConnectedUsers()
        {
            bool serverHasClients;
            Connection connection = clientProtocol.ConnectToServer();
            object[] request = BuildRequest(Command.ListOfConnectedUsers);
            connection.SendMessage(request);

            logger.LogAction(Command.ListOfConnectedUsers);

            var response = new Response(connection.ReadMessage());
            if (response.HadSuccess())
            {
                Console.WriteLine(ClientUI.TheseAreTheConnectedUsers());
                List<string> connectedClients = response.UserList();
                PrintUsers(connectedClients);
                serverHasClients = connectedClients.Count > 0;
            }
            else
            {
                Console.WriteLine(response.ErrorMessage());
                serverHasClients = false;
            }
            connection.Close();

            return serverHasClients;
        }

        private List<string> GetListOfAllClients()
        {
            var clients = new List<string>();
            Connection connection = clientProtocol.ConnectToServer();
            object[] request = BuildRequest(Command.ListOfAllClients);
            connection.SendMessage(request);

            logger.LogAction(Command.ListOfAllClients);

            var response = new Response(connection.ReadMessage());
            if (response.HadSuccess())
            {
                clients = response.UserList();
            }

            connection.Close();
            return clients;
        }

        private void SendFriendshipRequest()
        {
            List<string> clients = GetListOfAllClients();
            if (clients.Count == 0)
            {
                Console.WriteLine("There are no other registered clients");
                return;
            }
            Console.WriteLine("These are the connected users:");
            int input = Menus.MapInputWithMenuItemsList(clients);
            input--;
            string username = clients[input];
            object[] request = BuildRequest(Command.FriendshipRequest, username);

            Connection connection = clientProtocol.ConnectToServer();
            connection.SendMessage(request);

            logger.LogAction(Command.FriendshipRequest);

            var response = new Response(connection.ReadMessage());
            if (response.HadSuccess())
            {
                Console.WriteLine(response.ServerMessage());
            }
            else
            {
                Console.WriteLine(response.ErrorMessage());
            }
            connection.Close();
        }

        private void ListMyFriends()
        {
            PrintUsers(FriendsWithFriendsCountList());
        }

        private List<string> FriendsList()
        {
            var friends = new List<string>();
            Connection connection = clientProtocol.ConnectToServer();
            object[] request = BuildRequest(Command.ListMyFriends);
            connection.SendMessage(request);

            logger.LogAction(Command.ListMyFriends);

            var response = new Response(connection.ReadMessage());
            if (response.HadSuccess())
            {
                friends = response.UserList();
            }
            else
            {
                Console.WriteLine(response.ErrorMessage());
            }
            connection.Close();
            return friends;
        }

        private List<string> FriendsWithFriendsCountList()
        {
            var friends = new List<string>();
            Connection connection = clientProtocol.ConnectToServer();
            object[] request = BuildRequest(Command.ListMyFriends);
            connection.SendMessage(request);

            logger.LogAction(Command.ListMyFriends);

            var response = new Response(connection.ReadMessage());
            if (response.HadSuccess())
            {
                friends = response.UserWithFriendsCountList();
            }
            else
            {
                Console.WriteLine(response.ErrorMessage());
            }
            connection.Close();
            return friends;
        }

        private void RespondToFriendshipRequest()
        {
            string[][] requests = GetFriendshipRequests();
            if (requests.Length > 0)
            {
                string requestId = Menus.SelectRequest(requests);
                bool acceptRequest = Input.YesOrNo("Accept or reject this request");
                if (acceptRequest)
                {
                    AcceptFriendshipRequest(requestId);
                }
                else
                {
                    RejectFriendshipRequest(requestId);
                }
            }
            else
            {
                Console.WriteLine("No friendship requests");
            }
        }

        private List<string> MenuOptions()
        {
            return new List<string>(
                new[]
                {
                    "List Connected Users",
                    "List My Friends",
                    "Send Friendship Request",
                    "Respond to Friendship Request",
                    "Chat",
                    "Exit"
                });
        }

        private void AcceptFriendshipRequest(string requestId)
        {
            Connection conn = clientProtocol.ConnectToServer();
            conn.SendMessage(BuildRequest(Command.ConfirmFriendshipRequest, requestId));

            logger.LogAction(Command.ConfirmFriendshipRequest);

            var response = new Response(conn.ReadMessage());
            if (response.HadSuccess())
            {
                Console.WriteLine("Added " + response.GetUsername() + " as a friend");
            }
            else
            {
                Console.WriteLine(response.ErrorMessage());
            }
            conn.Close();
        }

        private void RejectFriendshipRequest(string requestId)
        {
            Connection conn = clientProtocol.ConnectToServer();
            conn.SendMessage(BuildRequest(Command.RejectFriendshipRequest, requestId));

            logger.LogAction(Command.RejectFriendshipRequest);

            var response = new Response(conn.ReadMessage());
            Console.WriteLine(response.HadSuccess() ? "Friendship request removed" : response.ErrorMessage());
            conn.Close();
        }

        private string[][] GetFriendshipRequests()
        {
            Connection connection = clientProtocol.ConnectToServer();
            connection.SendMessage(BuildRequest(Command.GetFriendshipRequests));

            logger.LogAction(Command.GetFriendshipRequests);

            var response = new Response(connection.ReadMessage());

            connection.Close();
            return response.FriendshipRequests();
        }

        private void PrintUsers(List<string> users)
        {
            users.ForEach(Console.WriteLine);
        }

        private void ConnectToServer()
        {
            Console.WriteLine(ClientUI.Connecting());
            bool connected;
            do
            {
                Business.Client client = AskForCredentials();
                Connection connection = clientProtocol.ConnectToServer();
                object[] request = BuildRequest(Command.Login, client.Username, client.Password);
                connection.SendMessage(request);
                var response = new Response(connection.ReadMessage());
                connected = response.HadSuccess();
                if (connected)
                {
                    clientToken = response.GetClientToken();
                    clientUsername = client.Username;
                    logger = new Logger(clientUsername, serverIp);
                    logger.LogAction(Command.Login);
                    Console.WriteLine(ClientUI.LoginSuccessful());
                }
                else
                {
                    Console.WriteLine(response.ErrorMessage());
                }
                connection.Close();
            } while (!connected);
        }

        private string GetServerIpFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (string) appSettings.GetValue("ServerIp", typeof(string));
        }

        private int GetServerPortFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (int) appSettings.GetValue("ServerPort", typeof(int));
        }

        private string GetClientIpFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (string) appSettings.GetValue("ClientIp", typeof(string));
        }

        private int GetClientPortFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (int) appSettings.GetValue("ClientPort", typeof(int));
        }

        private Business.Client AskForCredentials()
        {
            Console.WriteLine(ClientUI.LoginTitle());

            Console.WriteLine(ClientUI.InsertUsername());
            string username = Input.RequestString();

            Console.WriteLine(ClientUI.InsertPassword());
            string password = Input.RequestString();

            return new Business.Client(username, password);
        }

        private object[] BuildRequest(Command command, params object[] payload)
        {
            List<object> request = new List<object>(payload);
            request.Insert(0, new object[] {command.GetHashCode(), clientToken});

            return request.ToArray();
        }

        private void MapOptionToActionOfMainMenu(int option)
        {
            switch (option)
            {
                case 1:
                    ListConnectedUsers();
                    break;
                case 2:
                    ListMyFriends();
                    break;
                case 3:
                    SendFriendshipRequest();
                    break;
                case 4:
                    RespondToFriendshipRequest();
                    break;
                case 5:
                    Chat();
                    break;
                default:
                    DisconnectFromServer();
                    Environment.Exit(0);
                    break;
            }
        }

        private void Chat()
        {
            Console.WriteLine("Select a friend to send a message to:");
            List<string> friends = FriendsList();
            if (friends.Count == 0)
            {
                Console.WriteLine("You have no friends ¯\\_(ツ)_/¯");
                return;
            }
            int input = Menus.MapInputWithMenuItemsList(friends);
            input--;
            Console.Clear();
            Console.WriteLine("YOU ARE CHATTING WITH " + friends[input]);
            PrintConversations(friends[input]);

            Console.WriteLine("Type the content of your message:");
            string message = Input.RequestString();
            
            Console.WriteLine("You can leave the conversation typing 'exit'.");

            Connection connection = clientProtocol.ConnectToServer();
            object[] request = BuildRequest(Command.SendMessage, friends[input], message);
            connection.SendMessage(request);
            logger.LogAction(Command.SendMessage);

            var response = new Response(connection.ReadMessage());
            connection.Close();

            if (response.HadSuccess())
            {
                StartChat(friends[input]);
            }
            else
            {
                Console.WriteLine(response.ErrorMessage());
            }
        }

        private void PrintConversations(string friend)
        {
            Connection connection = clientProtocol.ConnectToServer();
            object[] request = BuildRequest(Command.GetConversation, friend);
            connection.SendMessage(request);

            logger.LogAction(Command.GetConversation);

            var response = new Response(connection.ReadMessage());
            connection.Close();

            if (response.HadSuccess())
            {
                response.Conversation(clientUsername).ForEach(Console.WriteLine);
            }
            else
            {
                Console.WriteLine(response.ErrorMessage());
            }
        }

        private void StartChat(string counterpartUsername)
        {
            var thread = new Thread(() => PrintWhatTheyWrite(counterpartUsername));
            thread.Start();
            while (true)
            {
                string myAnswer = Input.RequestString();

                if (myAnswer.Equals("exit"))
                    break;

                var messageSendingThread = new Thread(() => SendMessage(counterpartUsername, myAnswer));
                messageSendingThread.Start();
            }
            thread.Abort();
        }

        private void SendMessage(string counterpartUsername, string myAnswer)
        {
            try
            {
                Connection connection = clientProtocol.ConnectToServer();
                connection.SendMessage(BuildRequest(Command.SendMessage, counterpartUsername, myAnswer));

                logger.LogAction(Command.SendMessage);

                var sendMessageResponse = new Response(connection.ReadMessage());
                if (!sendMessageResponse.HadSuccess())
                    Console.WriteLine(sendMessageResponse.ErrorMessage());
                connection.Close();
            }
            catch (SocketException)
            {
                Console.WriteLine("Server is unreachable, app will exit.");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        private void PrintWhatTheyWrite(string counterpart)
        {
            try
            {
                int waitTime = InitialWaitTime;
                while (true)
                {
                    Connection connection = clientProtocol.ConnectToServer();
                    object[] readMessage = BuildRequest(Command.ReadMessage, counterpart);
                    connection.SendMessage(readMessage);

                    var readMessageResponse = new Response(connection.ReadMessage());
                    var messages = new List<string>();
                    if (readMessageResponse.HadSuccess())
                    {
                        messages = readMessageResponse.Messages();
                        if(messages.Count > 0)
                            logger.LogAction(Command.ReadMessage);
                        messages.ForEach(m => Console.WriteLine(counterpart + ": " + m));
                    }
                    else
                    {
                        Console.WriteLine(readMessageResponse.ErrorMessage());
                    }

                    if (messages.Count == 0)
                        waitTime = Convert.ToInt32(waitTime * WaitTimeAumentation);
                    else
                        waitTime = InitialWaitTime;
                    Thread.Sleep(waitTime);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Server is unreachable, app will exit.");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }
    }
}