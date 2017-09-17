using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Business;
using Protocol;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientRequestManager = new ClientProtocol();
            var clientController = new ClientController(clientRequestManager);
            clientController.Init();
        }
    }
}
