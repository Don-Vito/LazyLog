using LazyLog.LogProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LazyLog.ViewModel
{
    class FilterOption
    {
        public string Description { get; private set; }
        public Predicate<LogRecord> Predicate { get; set; }

        public FilterOption(string description, Predicate<LogRecord> predicate)
        {
            Description = description;
            Predicate = predicate;
        }
    }
}
