using System;
using System.ServiceModel;

namespace WcfServices
{
    public class WCFHost
    {
        private bool hostServiceIsRunning = false;

        public void Start()
        {
            using (var serviceHost = new ServiceHost(typeof(ClientCRUDService)))
            {
                serviceHost.Open();
                hostServiceIsRunning = true;
                while (hostServiceIsRunning) { }
                serviceHost.Close();
            }
        }

        public void Stop()
        {
            hostServiceIsRunning = false;
        }
    }
}