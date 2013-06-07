using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LazyLog.LogProviders
{
    public enum Severity
    {
        TraceEnter = 0,
        TraceLeave,
        TraceInfo,
        TraceError,
        Info,
        Error,
        Unexpected,
        Critical
    }
}
