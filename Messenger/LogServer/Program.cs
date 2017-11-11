using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LogServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string serverIp = GetServerIpFromConfigFile();
            var msmqServer = new MessageQueueServer(serverIp);
            var msmqServerThread = new Thread(() => msmqServer.Start());
            msmqServerThread.Start();

            var msmqClient = new MessageQueueClient(msmqServer);

            while (true)
            {
                Console.Clear();
                msmqClient.Menu();    
            }
        }

        private static string GetServerIpFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (string)appSettings.GetValue("ServerIp", typeof(string));
        }
    }
}
