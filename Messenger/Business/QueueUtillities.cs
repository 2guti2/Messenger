﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public static class QueueUtillities
    {
        public const string Name = "logQueue";
        private const string Localhost = "127.0.0.1";
        private const string ServerLogFileName = "ServerLog.txt";

        public static string Path(string serverIp)
        {
            return  serverIp.Equals(Localhost)
                    ? $".\\private$\\{Name}"
                    : $"FormatName:Direct=TCP:{serverIp}\\private$\\{Name}";
        }

        public static void SaveEntry(LogEntry entry)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(ServerLogFileName, true))
            {
                file.WriteLine(entry);
            }
        }
    }
}
