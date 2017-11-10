using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
