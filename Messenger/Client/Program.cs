using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UI;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                var clientController = new ClientController();
                clientController.LoopMenu();
            }
            catch (SocketException)
            {
                Console.WriteLine("There was a problem connecting to the server, the app will exit");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }
    }
}