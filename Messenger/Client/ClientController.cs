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
        private readonly ClientProtocol clientProtocol;
        private string clientToken;
        private string clientUsername;

        public ClientController()
        {
            clientToken = "";
            clientUsername = null;
            string serverIp = GetServerIpFromConfigFile();
            int serverPort = GetServerPortFromConfigFile();
            clientProtocol = new ClientProtocol(serverIp, serverPort);
        }

        internal void LoopMenu()
        {
            Init();
            while (true)
            {
                Console.WriteLine(ClientUI.Title(GetNotifications(), clientUsername));
                int option = Menus.MapInputWithMenuItemsList(MenuOptions());
                MapOptionToActionOfMainMenu(option);
                ClientUI.Clear();
            }
        }

        public void Init()
        {
            Console.WriteLine(ClientUI.Title());
            ConnectToServer();
            ClientUI.Clear();
        }

        public void ListConnectedUsers()
        {
            Connection connection = clientProtocol.ConnectToServer();
            object[] request = BuildRequest(Command.ListOfConnectedUsers);
            connection.SendMessage(request);

            var response = new Response(connection.ReadMessage());
            if (response.HadSuccess())
            {
                PrintUsers(response.UserList());
            }
            connection.Close();
        }

        public void SendFriendshipRequest()
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
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine(response.ErrorMessage());
            }
            connection.Close();
        }

        public void ListMyFriends()
        {
            PrintUsers(FriendsList());
        }

        public List<string> FriendsList()
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
            connection.Close();
            return friends;
        }

        public void AcceptFriendshipRequest()
        {
            string[][] requests = GetFriendshipRequests();
            if (requests.Length > 0)
            {
                string requestId = Menus.SelectRequest(requests);
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
            else
            {
                Console.WriteLine("No friendship requests");
            }
        }

        public void DisconnectFromServer()
        {
            Connection connection = clientProtocol.ConnectToServer();
            connection.SendMessage(BuildRequest(Command.DisconnectUser));
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
            users.ForEach(u => Console.WriteLine(u));
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

        public List<string> MenuOptions()
        {
            return new List<string>(
                new[]
                {
                    "List Connected Users",
                    "List My Friends",
                    "Send Friendship Request",
                    "Accept Friendship Request",
                    "Chat",
                    "Exit"
                });
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
                    AcceptFriendshipRequest();
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

        private List<int> GetNotifications()
        {
            var notifications = new List<int>();
            Connection connection = clientProtocol.ConnectToServer();
            object[] request = BuildRequest(Command.Notifications);
            connection.SendMessage(request);

            var response = new Response(connection.ReadMessage());
            if (response.HadSuccess())
            {
                notifications = response.Notifications();
            }
            connection.Close();
            return notifications;
        }

        private void Chat()
        {
            List<string> chatOptions = new List<string>(
                new[]
                {
                    "Start new conversation",
                    "View my current conversations"
                });

            int chatOptionsInput = Menus.MapInputWithMenuItemsList(chatOptions);

            switch (chatOptionsInput)
            {
                case 1:
                    StartNewConversation();
                    break;
                case 2:
                    ViewCurrentConversations();
                    break;
                default:
                    break;
            }
        }

        private void ViewCurrentConversations()
        {
            return;
            throw new NotImplementedException();
        }

        private void StartNewConversation()
        {
            Console.WriteLine("Select a friend to send a message to:");
            List<string> friends = FriendsList();

            int input = Menus.MapInputWithMenuItemsList(friends);
            input--;

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
        }

        private void StartChat(string counterpartUsername)
        {
            Console.WriteLine("talking");
            var talking = true;
            var thread = new Thread(() => PrintWhatTheyWrite(counterpartUsername));
            thread.Start();
            while (talking)
            {
                string myAnswer = Input.RequestString();

                Connection connection = clientProtocol.ConnectToServer();
                connection.SendMessage(BuildRequest(Command.SendMessage, counterpartUsername, myAnswer));
                var sendMessageResponse = new Response(connection.ReadMessage());
                if (!sendMessageResponse.HadSuccess())
                    Console.WriteLine(sendMessageResponse.ErrorMessage());
                connection.Close();
                if (myAnswer.Equals("exit"))
                    talking = false;
            }
        }

        private void PrintWhatTheyWrite(string counterpart)
        {
            while (true)
            {
                Connection connection = clientProtocol.ConnectToServer();
                object[] readMessage = BuildRequest(Command.ReadMessage, counterpart);
                connection.SendMessage(readMessage);

                var readMessageResponse = new Response(connection.ReadMessage());

                if (readMessageResponse.HadSuccess())
                    Console.WriteLine(counterpart + ": " + readMessageResponse.Message);

                Thread.Sleep(3000);
            }
        }
    }
}