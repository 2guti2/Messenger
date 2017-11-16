using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Log
{
    public class UploadFileEntry : LogEntryAttributes
    {
        public override string ToString()
        {
            return $"{Timestamp}: " + (ClientUsername ?? "Chat Server") + " uploaded a file.";
        }
    }
}
