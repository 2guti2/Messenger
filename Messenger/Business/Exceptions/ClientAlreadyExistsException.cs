using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Exceptions
{
    public class ClientAlreadyExistsException : BusinessException
    {
        public ClientAlreadyExistsException() : base("Client already exists") { }
    }
}
