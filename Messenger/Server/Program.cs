using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using Business;
using Persistence;
using Protocol;
using UI;
using WcfServices;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new ServerProtocol();
            int port = GetServerPortFromConfigFile();
            string ip = GetServerIpFromConfigFile();
            try
            {
                server.Start(ip, port);
            }
            catch (SocketException)
            {
                Console.WriteLine("There seems to be something else using the same port...");
                Console.ReadLine();
                Environment.Exit(0);
            }

            CoreController.Build(new Store());
            var businessController = CoreController.BusinessControllerInstance();

            var thread = new Thread(() =>
                {
                    var router = new Router(new ServerController(businessController));
                    while (true)
                    {
                        try
                        {
                            server.AcceptConnection(router.Handle);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("FAILED TO ACCEPT CONNECTION.");
                        }
                    }
                });
            thread.Start();

            var msmqServer = new MessageQueueServer(ip);
            var msmqServerThread = new Thread(() => msmqServer.Start());
            msmqServerThread.Start();

            WCFHost wcfHostService = new WCFHost();
            var wcfHostServiceThread = new Thread(() => wcfHostService.Start());
            wcfHostServiceThread.Start();

            var options = new List<string>(new[]
            {
                "Show All Clients",
                "Show Connected Clients",
                "Exit"
            });
            while (true)
            {
                int option = Menus.MapInputWithMenuItemsList(options);

                MapOptionToAction(option, businessController);

                if (option == options.Count)
                    break;
            }
            thread.Join();
            msmqServer.Stop();
            msmqServerThread.Join();
            wcfHostService.Stop();
            wcfHostServiceThread.Join();
        }

        private static void MapOptionToAction(int option, BusinessController controller)
        {
            if (option == 1)
                controller.GetClients().ForEach(client =>
                {
                    Console.WriteLine(
                        $"- {client.Username} \tFriends: {client.FriendsCount} \tConnected: {client.ConnectionsCount} times");
                });
            else if (option == 2)
            {
                controller.GetLoggedClients().ForEach(client =>
                {
                    if (client.ConnectedSince == null) return;
                    TimeSpan timeConnected = DateTime.Now.Subtract((DateTime)client.ConnectedSince);
                    string timeConnectedFormatted = timeConnected.ToString(@"hh\:mm\:ss");
                    Console.WriteLine(
                        $"- {client.Username} \tFriends: {client.FriendsCount} \tConnected: {client.ConnectionsCount} times \tConnected for: {timeConnectedFormatted}");
                });
            }
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
    }
}