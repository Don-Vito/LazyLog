using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LazyLog.ViewModel
{
    class FilterOption
    {
        public string Description { get; private set; }

        public FilterOption(string description)
        {
            Description = description;
        }        
    }
}
