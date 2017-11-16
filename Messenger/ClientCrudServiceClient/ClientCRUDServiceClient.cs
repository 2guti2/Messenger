using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using Business;

namespace ClientCrudServiceClient
{
    public class ClientCRUDServiceClient
    {
        private const string UsernameTakenErrorMessage = "\nUsername taken";
        private const string ThereAreNoClientsMessage = "There are no clients...";
        private List<string> menuOptions = new List<string>()
        {
            "Create", "Update", "Delete", "Exit"
        };

        private WcfServices.ClientCRUDServiceClient clientCrudServiceClient;

        public ClientCRUDServiceClient()
        {
            clientCrudServiceClient = new WcfServices.ClientCRUDServiceClient();
        }

        public void Menu()
        {
            Console.WriteLine("User CRUD Client");
            Console.WriteLine("----------------");
            int option = Menus.MapInputWithMenuItemsList(menuOptions);
            MapOptionToAction(option);
        }

        private void MapOptionToAction(int option)
        {
            switch (option)
            {
                case 1:
                    CreateClient();
                    break;
                case 2:
                    UpdateClient();
                    break;
                case 3:
                    DeleteClient();
                    break;
                default:
                    Environment.Exit(0);
                    return;
            }
        }

        private void DeleteClient()
        {
            try
            {
                ClientDto clientToDelete = AskExistingClientInfo();

                if(!clientCrudServiceClient.DeleteClient(clientToDelete))
                    Console.WriteLine("Client does not exist or is connected.");
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(ThereAreNoClientsMessage);
                Console.ReadKey();
            }
        }

        private void UpdateClient()
        {
            try
            {
                ClientDto existingClient = AskExistingClientInfo();

                ClientDto editedClient = AskNewClientInfo();

                clientCrudServiceClient.UpdateClient(existingClient, editedClient);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(ThereAreNoClientsMessage);
                Console.ReadKey();
            }
        }

        private ClientDto AskExistingClientInfo()
        {
            List<ClientDto> existingClients = clientCrudServiceClient.GetClients().ToList();

            if(existingClients.Count == 0)
                throw new IndexOutOfRangeException();

            var existingClientsUsernames = new List<string>();

            existingClients.ForEach(ec => existingClientsUsernames.Add(ec.Username));

            int option = Menus.MapInputWithMenuItemsList(existingClientsUsernames);
            option--;

            string selectedClientUsername = existingClientsUsernames[option];

            return existingClients.Find(ec => ec.Username.Equals(selectedClientUsername));
        }

        private void CreateClient()
        {
            bool created = false;

            while (!created)
            {
                ClientDto clientToCreate = AskNewClientInfo();

                created = clientCrudServiceClient.CreateClient(clientToCreate);

                if (!created)
                    Console.WriteLine(UsernameTakenErrorMessage);
            }
        }

        private ClientDto AskNewClientInfo()
        {
            Console.WriteLine("Insert username:");
            string username = Input.RequestString();

            Console.WriteLine("Insert Password");
            string password = Input.RequestString();

            return new ClientDto()
            {
                Username = username,
                Password = password
            };
        }
    }
}
