using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LazyLog.LogProviders
{
    public interface ILogParser
    {
        IEnumerable<LogRecord> Parse(string data);
    }
}
