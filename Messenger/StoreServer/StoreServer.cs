using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Persistence;
using Business;

namespace StoreServer
{
    class StoreServer
    {
        public void Start()
        {
            var serverChannel = new TcpChannel(GetServerPortFromConfigFile());
            try
            {
                ChannelServices.RegisterChannel(serverChannel, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(Store), StoreUtillities.StoreName, WellKnownObjectMode.Singleton);
                Console.WriteLine("Store server started, click any key to stop.");
                Console.ReadLine();
                ChannelServices.UnregisterChannel(serverChannel);
            }
            catch (Exception)
            {
                ChannelServices.UnregisterChannel(serverChannel);
            }
        }

        private static int GetServerPortFromConfigFile()
        {
            var appSettings = new AppSettingsReader();
            return (int)appSettings.GetValue("ServerPort", typeof(int));
        }
    }
}
