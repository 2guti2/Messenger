using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServices;
using Business;
using Persistence;

namespace ClientCrudServiceServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string storeServerIp = GetStoreServerIpFromConfigFile();
            int storeServerPort = GetStoreServerPortFromConfigFile();
            var store = (Store)Activator.GetObject(typeof(Store), $"tcp://{storeServerIp}:{storeServerPort}/{StoreUtillities.StoreName}");
            CoreController.Build(store);

            WCFHost wcfHostService = new WCFHost();
            wcfHostService.Start();
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
