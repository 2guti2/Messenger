using System;
using System.Collections.Generic;
using Protocol;
using UI;
using System.Configuration;
using Business;

namespace Client
{
    public class ClientController
    {
        private readonly ClientProtocol clientProtocol;
        private string clientToken;

        public ClientController()
        {
            clientToken = "";
            string serverIp = GetServerIpFromConfigFile();
            int serverPort = GetServerPortFromConfigFile();
            clientProtocol = new ClientProtocol(serverIp, serverPort);
        }

        public void Init()
        {
            Console.WriteLine(ClientUI.Title());
            Console.WriteLine(ClientUI.CallToAction());
            Console.ReadKey();

            ConnectToServer();
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
        }

        public void SendFriendshipRequest()
        {
            Connection connection = clientProtocol.ConnectToServer();

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
        }

        public void AcceptFriendshipRequest()
        {
            string[][] requests = GetFriendshipRequests();
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
        }

        private string[][] GetFriendshipRequests()
        {
            Connection connection = clientProtocol.ConnectToServer();
            connection.SendMessage(BuildRequest(Command.GetFriendshipRequests));
            var response = new Response(connection.ReadMessage());

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
                    Console.WriteLine(ClientUI.LoginSuccessful());
                }
                else
                {
                    Console.WriteLine(ClientUI.InvalidCredentials());
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

        public void PrintMenu()
        {
            Console.WriteLine("Menu goes here");
        }
    }
}