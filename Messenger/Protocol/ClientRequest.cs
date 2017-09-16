using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business;

namespace Protocol
{
    public class ClientRequest : IClientRequest
    {

        public ClientRequest() { }

        public bool ConnectToServer(string serverIp, Client client)
        {
            return false;
        }
    }
}
