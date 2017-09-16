﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class BussinessController
    {
        private IStore Store { get; set; }
        private Server Server { get; set; }

        public BussinessController(IStore store)
        {
            Store = store;
            Server = new Server();
        }

        public void Login(Client client)
        {
            if (!Store.ClientExists(client))
            {
                Store.AddClient(client);
            }
            Server.ConnectClient(client);
        }
    }
}
