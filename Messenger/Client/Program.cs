using Protocol;
using System;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientController = new ClientController();
            clientController.Init();
        }
    }
}
