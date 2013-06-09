using LazyLog.LogProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LazyLog.ViewModel
{
    public class FilterOption
    {
        public string Description { get; private set; }
        public Predicate<LogRecord> Predicate { get; set; }

        public FilterOption(string description, Predicate<LogRecord> predicate)
        {
            Description = description;
            Predicate = predicate;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {           
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            FilterOption other = obj as FilterOption;
            return Description.Equals(other.Description);    
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {           
            return Description.GetHashCode();
        }
    }
}
