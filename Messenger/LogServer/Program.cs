using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Business;
using Persistence;

namespace LogServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string storeServerIp = Utillities.GetStoreServerIpFromConfigFile();
            int storeServerPort = Utillities.GetStoreServerPortFromConfigFile();
            var store =
                (Store)Activator.GetObject
                (
                    typeof(Store),
                    $"tcp://{storeServerIp}:{storeServerPort}/{StoreUtillities.StoreName}"
                );
            CoreController.Build(store);
            BusinessController businessController = CoreController.BusinessControllerInstance();

            var msmqServer = new MessageQueueServer(businessController);
            var msmqServerThread = new Thread(() => msmqServer.Start());
            msmqServerThread.Start();

            var msmqClient = new MessageQueueClient(msmqServer, businessController);

            Console.Clear();
            while (true)
            {
                msmqClient.PrintLogs();    
            }
        }
    }
}
