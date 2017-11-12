using System;

namespace Business
{
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
