using System;
using System.Threading;
using Business;
using Persistence;

namespace LogClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string storeServerIp = Utillities.GetStoreServerIpFromConfigFile();
            int storeServerPort = Utillities.GetStoreServerPortFromConfigFile();
            string logServerIp = Utillities.GetLogServerIpFromConfigFile();

            Store store = null;
            try
            {
                store = (Store)Activator.GetObject(typeof(Store),
                    $"tcp://{storeServerIp}:{storeServerPort}/{StoreUtillities.StoreName}");
                store.GetClients();
            }
            catch (Exception)
            {
                Console.WriteLine("Store isn't available. Closing app...");
                Thread.Sleep(5000);
                Environment.Exit(0);
            }

            CoreController.Build(store);
            BusinessController bc = CoreController.BusinessControllerInstance();

            var msmqClient = new MessageQueueClient(bc);
            while (true)
            {
                Console.Clear();
                msmqClient.Menu();
            }
        }
    }
}
