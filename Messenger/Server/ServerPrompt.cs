using System;
using System.Collections.Generic;
using Business;
using UI;

namespace Server
{
    public class ServerPrompt
    {
        private BusinessController controller;
        private readonly FileManager fileManager = new FileManager();

        public ServerPrompt(BusinessController businessController)
        {
            controller = businessController;
        }

        public void PromptUserForAction()
        {
            while (true)
            {
                int option = Menus.MapInputWithMenuItemsList(Options());

                MapOptionToAction(option);

                if (option == Options().Count)
                    break;
            }
        }

        private List<string> Options()
        {
            return new List<string>(new[]
            {
                "Show All Clients",
                "Show Connected Clients",
                "Upload File",
                "Download File",
                "Exit"
            });
        }

        private void MapOptionToAction(int option)
        {
            switch (option)
            {
                case 1:
                    ListAllClients();
                    break;
                case 2:
                    ListConnectedClients();
                    break;
                case 3:
                    fileManager.UploadFile();
                    break;
                case 4:
                    fileManager.DownloadFile();
                    break;
            }
        }

        private void ListAllClients()
        {
            controller.GetClients().ForEach(client =>
            {
                Console.WriteLine(
                    $"- {client.Username} \tFriends: {client.FriendsCount} \tConnected: {client.ConnectionsCount} times");
            });
        }

        private void ListConnectedClients()
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
}