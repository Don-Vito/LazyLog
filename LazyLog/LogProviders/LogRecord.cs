using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LazyLog.LogProviders
{
    public class LogRecord
    {
        public DateTime TimeStamp { get; private set; }
        public Severity Severity { get; private set; }
        public string ModuleName { get; private set; }
        public string Message { get; private set; }
        public string ProcessId { get; private set; }
        public string ThreadId { get; private set; }

        public LogRecord(DateTime timeStamp, Severity severity, string moduleName, string message, string processId, string threadId)
        {
            TimeStamp = timeStamp;
            Severity = severity;
            ModuleName = moduleName;
            Message = message;
            ProcessId = processId;
            ThreadId = threadId;
        }
    }
}
