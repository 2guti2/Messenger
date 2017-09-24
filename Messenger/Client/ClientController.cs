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

        public void SendFriendshipRequest()
        {
            Connection connection = clientProtocol.ConnectToServer();

            Console.WriteLine(ClientUI.PromptUsername());
            string username = Console.ReadLine();
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

        private void ConnectToServer()
        {
            Console.WriteLine(ClientUI.Connecting());
            bool connected;
            do
            {
                Connection connection = clientProtocol.ConnectToServer();
                Business.Client client = AskForCredentials();
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
            string username = Console.ReadLine();

            Console.WriteLine(ClientUI.InsertPassword());
            string password = Console.ReadLine();

            return new Business.Client(username, password);
        }

        private object[] BuildRequest(Command command, params object[] payload)
        {
            var request = new List<object>(payload);
            request.Insert(0, new object[] {command.GetHashCode(), clientToken});

            return request.ToArray();
        }
    }
}