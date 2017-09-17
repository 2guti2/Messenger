using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class Response <T>
    {
        public Response() { }

        public Response(int code, T body)
        {
            Code = code;
            Body = body;
        }
        
        public int Code { get; set; }
        public T Body { get; set; }
    }
}
