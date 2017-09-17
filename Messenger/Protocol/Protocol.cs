using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    /*
        using Newtonsoft.Json;
        Client client = new Client("2guti2", "123456");
        string json = JsonConvert.SerializeObject(client);
        Console.WriteLine(json);
        Console.Read();
        Client tmp = JsonConvert.DeserializeObject<Client>(json);
    */
    public abstract class Protocol
    {
        protected void ReadData(Socket socket)
        {
            var buffer = new byte[100000];
            int iRx = socket.Receive(buffer);
            var chars = new char[iRx];

            Decoder d = Encoding.UTF8.GetDecoder();
            int charLen = d.GetChars(buffer, 0, iRx, chars, 0);

            Console.WriteLine("Message: " + new string(chars));
        }
    }
}
