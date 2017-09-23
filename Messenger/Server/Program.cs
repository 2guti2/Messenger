using System;
using System.Threading;
using Business;
using Persistence;
using Protocol;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new ServerProtocol();
            server.Start("127.0.0.1", 6500);
            while (true)
            {
                server.AcceptConnection(Router.Handle);
            }
        }
    }
}
