using System.Configuration;
using System.Threading;
using Business;
using Persistence;
using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = GetServerPortFromConfigFile();
            string ip = GetServerIpFromConfigFile();

            string storeServerIp = GetStoreServerIpFromConfigFile();
            int storeServerPort = GetStoreServerPortFromConfigFile();

            //TODO:MG try catch this
            var store = (Store)Activator.GetObject(typeof(Store), $"tcp://{storeServerIp}:{storeServerPort}/{StoreUtillities.StoreName}");

            CoreController.Build(store);
            BusinessController businessController = CoreController.BusinessControllerInstance();

            var launcher = new ServerLauncher(ip, port);
            launcher.Launch();
            Thread serverThread = launcher.StartAcceptingConnections(businessController);

            var prompt = new ServerPrompt(businessController);
            prompt.PromptUserForAction();

            serverThread.Join();
        }

        private static string GetServerIpFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (string)appSettings.GetValue("ServerIp", typeof(string));
        }

        private static int GetServerPortFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (int)appSettings.GetValue("ServerPort", typeof(int));
        }

        private static string GetStoreServerIpFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (string)appSettings.GetValue("StoreServerIp", typeof(string));
        }

        private static int GetStoreServerPortFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (int)appSettings.GetValue("StoreServerPort", typeof(int));
        }
    }
}