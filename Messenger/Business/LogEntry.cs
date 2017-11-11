﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{   //TODO:MG make this class abstract and create implementations for every kind of log entry
    [Serializable]
    public class LogEntry
    {
        public string ClientUsername { get; set; }
        public DateTime Timestamp { get; set; }
        public Command Action { get; set; }

        public override string ToString()
        {
            return $"{Timestamp}: {ClientUsername} - {Action}";
        }
    }
}
