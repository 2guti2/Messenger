using System;
using Protocol;
using UI;
using System.Configuration;

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

        private void ConnectToServer()
        {
            Console.WriteLine(ClientUI.Connecting());
            var connected = false;
            do
            {
                var connection = clientProtocol.ConnectToServer();
                Business.Client client = AskForCredentials();
                object[] request = {Command.Login.GetHashCode(), client.Username, client.Password};
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
    }
}