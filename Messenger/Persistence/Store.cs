using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Business;
using System.Threading.Tasks;

namespace Persistence
{
    public class Store : IStore
    {
        public Store()
        {

        }

        public List<Client> Clients { get; set; }
    }
}
