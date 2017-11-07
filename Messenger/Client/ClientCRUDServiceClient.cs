using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business;
using UI;

namespace Client
{
    public class ClientCRUDServiceClient
    {
        private const string UsernameTakenErrorMessage = "\nUsername taken";

        private Logger logger;
        private WcfServices.ClientCRUDServiceClient clientCrudServiceClient;
        private List<string> menuOptions = new List<string>()
        {
            "Create", "Update", "Delete", "Exit"
        };

        public ClientCRUDServiceClient(Logger logger)
        {
            this.logger = logger;
            clientCrudServiceClient = new WcfServices.ClientCRUDServiceClient();
        }

        public void Menu()
        {
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
                    return;
            }
        }

        private void DeleteClient()
        {
            ClientDto clientToDelete = AskExistingClientInfo();

            clientCrudServiceClient.DeleteClient(clientToDelete);

            logger.LogAction(Command.DeleteUser);
        }

        private void UpdateClient()
        {
            ClientDto existingClient = AskExistingClientInfo();

            ClientDto editedClient = AskNewClientInfo();

            clientCrudServiceClient.UpdateClient(existingClient, editedClient);

            logger.LogAction(Command.UpdateUser);
        }

        private ClientDto AskExistingClientInfo()
        {
            List<ClientDto> existingClients = clientCrudServiceClient.GetClients().ToList();

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

            logger.LogAction(Command.CreateUser);
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
