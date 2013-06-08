using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LazyLog.LogProviders
{
    public class LogRecord
    {
        private static int currentID = 0;

        public int ID { get; private set; }
        public DateTime TimeStamp { get; private set; }
        public Severity Severity { get; private set; }
        public string ModuleName { get; private set; }
        public string Message { get; private set; }
        public string ProcessId { get; private set; }
        public string ThreadId { get; private set; }

        public static LogRecord Create(DateTime timeStamp, Severity severity, string moduleName, string message, string processId, string threadId)
        {
            return new LogRecord(currentID++, timeStamp, severity, moduleName, message, processId, threadId);
        }

        private LogRecord(int id, DateTime timeStamp, Severity severity, string moduleName, string message, string processId, string threadId)
        {
            ID = id;
            TimeStamp = timeStamp;
            Severity = severity;
            ModuleName = moduleName;
            Message = message;
            ProcessId = processId;
            ThreadId = threadId;
        }
    }
}
