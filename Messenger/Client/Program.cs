using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UI;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var clientController = new ClientController();
                clientController.Init();
                while (true)
                {
                    int option = Menus.MainMenu(MenuOptions());
                    MapOptionToAction(option, clientController);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("There was a problem connecting to the server, the app will exit");
                Environment.Exit(1);
            }
        }

        private static List<string> MenuOptions()
        {
            return new List<string>(
                new[]
                {
                    "List Connected Users",
                    "Send Friendship Request",
                    "Send Friendship Request",
                    "Accept Friendship Request",
                    "Exit"
                });
        }

        private static void MapOptionToAction(int option, ClientController clientController)
        {
            switch (option)
            {
                case 1:
                    clientController.ListConnectedUsers();
                    break;
                case 2:
                    clientController.SendFriendshipRequest();
                    break;
                case 3:
                    clientController.SendFriendshipRequest();
                    break;
                case 4:
                    clientController.AcceptFriendshipRequest();
                    break;
                default:
                    Environment.Exit(0);
                    break;
            }
        }
    }
}