using System;
using WcfServices;
using Business;
using Persistence;
using System.Threading;

namespace ClientCrudServiceServer
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

            WCFHost wcfHostService = new WCFHost();
            var wcfHostServiceThread = new Thread((() => wcfHostService.Start()));
            wcfHostServiceThread.Start();

            Console.WriteLine("Client CRUD Service Server running. Click any key to stop...");
            Console.ReadKey();

            wcfHostService.Stop();
            wcfHostServiceThread.Abort();
        }
    }
}
