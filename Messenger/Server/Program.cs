using System.Configuration;
using System.Threading;
using Business;
using Persistence;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string ip = GetServerIpFromConfigFile();
            int port = GetServerPortFromConfigFile();
            var businessController = new BusinessController(new Store());

            var launcher = new ServerLauncher(ip, port);
            launcher.Launch();
            Thread serverThread = launcher.StartAcceptingConnections(businessController);

            var msmqServer = new MessageQueueServer(ip);
            var msmqServerThread = new Thread(() => msmqServer.Start());
            msmqServerThread.Start();
            
            var prompt = new ServerPrompt(businessController);
            prompt.PromptUserForAction();

            serverThread.Join();
        }

        private static string GetServerIpFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (string) appSettings.GetValue("ServerIp", typeof(string));
        }

        private static int GetServerPortFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (int) appSettings.GetValue("ServerPort", typeof(int));
        }
    }
}