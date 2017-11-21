using System;
using System.Collections.Generic;
using Business;
using UI;

namespace LogClient
{
    internal class MessageQueueClient
    {
        private BusinessController businessController;
        private List<string> menuOptions = new List<string>()
        {
            "Print Logs", "Exit"
        };

        public MessageQueueClient(BusinessController businessController)
        {
            this.businessController = businessController;
        }

        public void Menu()
        {
            Console.WriteLine("LOG CLIENT");
            Console.WriteLine("----------");
            int option = Menus.MapInputWithMenuItemsList(menuOptions);
            MapOptionToAction(option);
        }

        private void MapOptionToAction(int option)
        {
            switch (option)
            {
                case 1:
                    List<LogEntry> logs = businessController.GetLogEntries();
                    logs.ForEach(e => Console.WriteLine(e));
                    Console.WriteLine("-------------------");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;
                default:
                    Environment.Exit(0);
                    break;
            }
        }
    }
}
