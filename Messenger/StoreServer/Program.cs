using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var storeServer = new StoreServer();
            storeServer.Start();
        }
    }
}
