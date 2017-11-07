using Business;

namespace WcfServices
{
    public class Adapter
    {
        public static Client ClientDtoToClient(ClientDto clientDto)
        {
            return new Client(clientDto.Username, clientDto.Password);
        }

        public static ClientDto ClientToClientDto(Client client)
        {
            return new ClientDto()
            {
                Username = client.Username,
                Password = client.Password
            };
        }
    }
}
