using System.Collections.Generic;

namespace LazyLog.LogProviders
{
    public interface ILogParser
    {
        IEnumerable<LogRecord> Parse(string data);
    }
}
