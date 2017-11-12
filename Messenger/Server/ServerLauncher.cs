﻿using System;
using System.Threading;
using Business;
using Protocol;

namespace Server
{
    public class ServerLauncher
    {
        private ServerProtocol server;
        private string serverIp;
        private int serverPort;
        
        public ServerLauncher(string serverIp, int serverPort)
        {
            this.serverIp = serverIp;
            this.serverPort = serverPort;
            server = new ServerProtocol();
        }

        public void Launch()
        {
            server.Start(serverIp, serverPort);
        }

        public Thread StartAcceptingConnections(BusinessController businessController)
        {
            var thread = new Thread(() =>
            {
                var router = new Router(new ServerController(businessController));
                while (true)
                {
                    try
                    {
                        server.AcceptConnection(router.Handle);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("FAILED TO ACCEPT CONNECTION.");
                    }
                }
            });
            thread.Start();
            return thread;
        }
    }
}