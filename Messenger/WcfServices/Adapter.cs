using Business;

namespace WcfServices
{
    public class Adapter
    {
        public static bool Equals(ClientDto clientDto, Client client)
        {
            return clientDto.Username.Equals(client.Username) && clientDto.Password.Equals(client.Password);
        }
    }
}
