using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sapzeugnisablage
{
    public class LogEntry
    {
        public string Message { get; set; }
        public DateTime DateTimeCreated { get; set; }
        LogEntry(string message)
        {
            Message = message;
            DateTimeCreated = DateTime.Now;
        }
    }
}
