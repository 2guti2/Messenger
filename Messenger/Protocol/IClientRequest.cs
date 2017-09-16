using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business;

namespace Protocol
{
    public interface IClientRequest
    {
        bool ConnectToServer(string serverIp, Client client);
    }
}
