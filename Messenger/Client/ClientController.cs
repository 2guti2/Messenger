using System;
using System.Collections.Generic;
using Protocol;
using UI;
using System.Configuration;
using Business;
using System.Threading;


namespace Client
{
    public class ClientController
    {
        private const double WaitTimeAumentation = 1.5;
        private const int InitialWaitTime = 100;
        private readonly ClientProtocol clientProtocol;
        private string clientToken;
        private string clientUsername;

        public ClientController()
        {
            clientToken = "";
            clientUsername = null;
            string serverIp = GetServerIpFromConfigFile();
            int serverPort = GetServerPortFromConfigFile();
            string clientIp = GetClientIpFromConfigFile();
            int clientPort = GetClientPortFromConfigFile();
            clientProtocol = new ClientProtocol(serverIp, serverPort, clientIp, clientPort);
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

        private void ListConnectedUsers()
        {
            Connection connection = clientProtocol.ConnectToServer();
            object[] request = BuildRequest(Command.ListOfConnectedUsers);
            connection.SendMessage(request);

            var response = new Response(connection.ReadMessage());
            if (response.HadSuccess())
            {
                PrintUsers(response.UserList());
            }
            else
            {
                Console.WriteLine(response.ErrorMessage());
            }
            connection.Close();
        }

        private void SendFriendshipRequest()
        {
            Connection connection = clientProtocol.ConnectToServer();
            Console.WriteLine(ClientUI.TheseAreTheConnectedUsers());
            ListConnectedUsers();
            Console.WriteLine(ClientUI.PromptUsername());
            string username = Input.RequestString();
            object[] request = BuildRequest(Command.FriendshipRequest, username);

            connection.SendMessage(request);
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

        public void DisconnectFromServer()
        {
            Connection connection = clientProtocol.ConnectToServer();
            connection.SendMessage(BuildRequest(Command.DisconnectUser));
        }

        private void AcceptFriendshipRequest(string requestId)
        {
            Connection conn = clientProtocol.ConnectToServer();
            conn.SendMessage(BuildRequest(Command.ConfirmFriendshipRequest, requestId));
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
            var response = new Response(conn.ReadMessage());
            Console.WriteLine(response.HadSuccess() ? "Friendship request removed" : response.ErrorMessage());
            conn.Close();
        }

        private string[][] GetFriendshipRequests()
        {
            Connection connection = clientProtocol.ConnectToServer();
            connection.SendMessage(BuildRequest(Command.GetFriendshipRequests));
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

        public void MapOptionToActionOfMainMenu(int option)
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

            int input = Menus.MapInputWithMenuItemsList(friends);
            input--;

            PrintConversations(friends[input]);

            Console.WriteLine("Type the content of your message:");
            string message = Console.ReadLine();

            Connection connection = clientProtocol.ConnectToServer();
            object[] request = BuildRequest(Command.SendMessage, friends[input], message);
            connection.SendMessage(request);

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
            Connection connection = clientProtocol.ConnectToServer();
            connection.SendMessage(BuildRequest(Command.SendMessage, counterpartUsername, myAnswer));
            var sendMessageResponse = new Response(connection.ReadMessage());
            if (!sendMessageResponse.HadSuccess())
                Console.WriteLine(sendMessageResponse.ErrorMessage());
            connection.Close();
        }

        private void PrintWhatTheyWrite(string counterpart)
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
    }
}