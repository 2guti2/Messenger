using System;
using System.ServiceModel;

namespace WcfServices
{
    public class Host
    {
        public void Start()
        {
            using (var serviceHost = new ServiceHost(typeof(ClientCRUDService)))
            {
                Console.WriteLine("Starting service...");
                serviceHost.Open();
                Console.WriteLine("Service is running, press return to stop");
                Console.ReadLine();
            }
        }
    }
}
