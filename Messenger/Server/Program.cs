using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business;
using Persistence;
using Protocol;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var store = new Store();
            var bussinessController = new BussinessController(store);
            var server = new ServerProtocol();
            server.Start("127.0.0.1", 6500);
            while (true)
            {
                server.AcceptRequest();
            }
        }
    }
}
