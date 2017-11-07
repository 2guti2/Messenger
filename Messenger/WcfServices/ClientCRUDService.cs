using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Business;

namespace WcfServices
{
    public class ClientCRUDService : IClientCRUDService
    {
        private readonly BusinessController businessController;

        public ClientCRUDService()
        {
            businessController = CoreController.BusinessControllerInstance();
        }

        public bool CreateClient(ClientDto clientDto)
        {
            Client client = Adapter.ClientDtoToClient(clientDto);

            return businessController.CreateClient(client);
        }

        public bool DeleteClient(ClientDto clientDto)
        {
            Client client = Adapter.ClientDtoToClient(clientDto);

            return businessController.DeleteClient(client);
        }

        public List<ClientDto> GetClients()
        {
            List<ClientDto> clientDtos = new List<ClientDto>();

            businessController.GetClients().ForEach(c => clientDtos.Add(Adapter.ClientToClientDto(c)));

            return clientDtos;
        }

        public bool UpdateClient(ClientDto existingClientDto, ClientDto newClientDto)
        {
            Client existingClient = Adapter.ClientDtoToClient(existingClientDto);
            Client newClient = Adapter.ClientDtoToClient(newClientDto);

            return businessController.UpdateClient(existingClient, newClient);
        }
    }
}