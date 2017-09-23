using System;
using Protocol;
using UI;
using System.Configuration;
using Business;

namespace Client
{
    public class ClientController
    {
        private ClientProtocol clientProtocol;
        private string clientToken;

        public ClientController()
        {
            string serverIp = GetServerIpFromConfigFile();
            int serverPort = GetServerPortFromConfigFile();
            this.clientProtocol = new ClientProtocol(serverIp, serverPort);
        }

        public void Init()
        {
            Console.WriteLine(ClientUI.Title());
            Console.WriteLine(ClientUI.CallToAction());
            Console.ReadKey();

            ConnectToServer();
        }

        public void ConnectToServer()
        {
            Console.WriteLine(ClientUI.Connecting());
            var connection = clientProtocol.ConnectToServer();

            var connected = false;
            do
            {
                Business.Client client = AskForCredentials();
                object[] request = {Command.Login.GetHashCode(), client.Username, client.Password};
                connection.SendMessage(request);
                var response = new Response(connection.ReadMessage());
                connected = response.HadSuccess();
                if (connected)
                    clientToken = response.GetClientToken();
                else
                    Console.WriteLine(ClientUI.InvalidCredentials());
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
    }
}
