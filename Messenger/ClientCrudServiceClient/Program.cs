using System;

namespace ClientCrudServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientCrudServiceClient = new ClientCRUDServiceClient();

            while (true)
            {
                Console.Clear();
                clientCrudServiceClient.Menu();
            }
        }
    }
}
