using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            IClientRequest clientRequestManager = new ClientRequest();
            var clientController = new ClientController(clientRequestManager);
            clientController.Init();
        }
    }
}
