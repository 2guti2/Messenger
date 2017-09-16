using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client;

namespace Protocol
{
    public class ClientRequest : IClientRequest
    {
        private IServerRequest serverRequestManager;

        public ClientRequest(IServerRequest serverRequestManager)
        {
            this.serverRequestManager = serverRequestManager;
        }

        public bool ConnectToServer()
        {
            return false;
        }
    }
}
