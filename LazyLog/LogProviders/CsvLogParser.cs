using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyLog.LogProviders
{
    public class CsvLogParser : ILogParser
    {
        public IEnumerable<LogRecord> Parse(string data)
        {
            IEnumerable<string> lines = data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            return lines.Select(line => ParseLine(line));
        }

        private static LogRecord ParseLine(string line)
        {
            string[] values = line.Split(new[] { ',' });

            return LogRecord.Create(
                DateTime.ParseExact(values[0], "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture),
                (Severity)Enum.Parse(typeof(Severity), values[1]),
                values[2],  //module name
                values[3],  //message
                values[4],  //process ID
                values[5]   //thread ID
                );
        }
    }
}
