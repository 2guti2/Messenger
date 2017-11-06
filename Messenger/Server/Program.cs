using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using Business;
using Persistence;
using Protocol;
using UI;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new ServerProtocol();
            int port = GetServerPortFromConfigFile();
            string ip = GetServerIpFromConfigFile();
            server.Start(ip, port);
            var businessController = new BusinessController(new Store());

            var thread = new Thread(() =>
            {
                var router = new Router(new ServerController(businessController));
                while (true)
                    server.AcceptConnection(router.Handle);
            });
            thread.Start();

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
                    TimeSpan timeConnected = DateTime.Now.Subtract((DateTime) client.ConnectedSince);
                    string timeConnectedFormatted = timeConnected.ToString(@"hh\:mm\:ss");
                    Console.WriteLine(
                        $"- {client.Username} \tFriends: {client.FriendsCount} \tConnected: {client.ConnectionsCount} times \tConnected for: {timeConnectedFormatted}");
                });
            }
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