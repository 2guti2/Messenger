using System;
using System.Collections.Generic;
using System.Configuration;
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
        private LogRouter logRouter;

        public ClientCRUDService()
        {
            businessController = CoreController.BusinessControllerInstance();
            logRouter = new LogRouter();
        }

        public bool CreateClient(ClientDto clientDto)
        {
            Client client = Adapter.ClientDtoToClient(clientDto);

            bool result = businessController.CreateClient(client);

            if(result)
                logRouter.LogAction(CrudAction.Create, client);

            return result;
        }

        public bool DeleteClient(ClientDto clientDto)
        {
            Client client = Adapter.ClientDtoToClient(clientDto);

            bool result = businessController.DeleteClient(client);

            if(result)
                logRouter.LogAction(CrudAction.Delete, client);

            return result;
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

            bool result = businessController.UpdateClient(existingClient, newClient);

            if(result)
                logRouter.LogAction(CrudAction.Update, existingClient);

            return result;
        }
    }
}