﻿using System;
using Protocol;
using UI;
using System.Configuration;

namespace Client
{
    public class ClientController
    {
        private ClientProtocol clientRequestManager;
        private string clientToken;

        public ClientController(ClientProtocol clientRequestManager)
        {
            this.clientRequestManager = clientRequestManager;
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
            string serverIp = GetServerIpFromConfigFile();

            bool connected = false;
            do
            {
                Business.Client client = AskForCredentials();
                connected = true;
                var connection = clientRequestManager.ConnectToServer();
            } while (!connected);
        }

        private string GetServerIpFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (string)appSettings.GetValue("ServerIp", typeof(string));
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
