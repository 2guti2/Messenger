using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business;

namespace Protocol
{
    public class ClientProtocol : Protocol
    {

        public ClientProtocol() { }

        public bool ConnectToServer(string serverIp, Business.Client client)
        {
            return false;
        }
    }
}
