using System;
using System.Collections.Generic;
using Protocol;
using UI;
using System.Configuration;

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
                Console.WriteLine(ClientUI.Title(clientUsername));
                int option = Menus.MainMenu(MenuOptions());
                MapOptionToAction(option);
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
            Connection connection = clientProtocol.ConnectToServer();
            object[] request = BuildRequest(Command.ListMyFriends);
            connection.SendMessage(request);

            var response = new Response(connection.ReadMessage());
            if (response.HadSuccess())
            {
                PrintUsers(response.UserList());
            }
            connection.Close();
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

        private void DisconnectFromServer()
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

        public void MapOptionToAction(int option)
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
            throw new NotImplementedException();
        }
    }
}