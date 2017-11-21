using System.Collections.Generic;
using System.ServiceModel;
using Business;

namespace WcfServices
{
    [ServiceContract]
    public interface IClientCRUDService
    {
        [OperationContract]
        bool CreateClient(ClientDto client);

        [OperationContract]
        bool UpdateClient(ClientDto existingClientDto, ClientDto newClientDto);

        [OperationContract]
        List<ClientDto> GetClients();

        [OperationContract]
        bool DeleteClient(ClientDto client);
    }
}
