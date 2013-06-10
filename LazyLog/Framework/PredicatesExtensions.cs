using System;
using System.Collections.Generic;
using System.Linq;

namespace LazyLog.Framework
{
    public static class PredicatesExtensions
    {
        public static Predicate<T> And<T>(IEnumerable<Predicate<T>> predicates)
        {
            return item => predicates.All(predicate => predicate(item));
        }
    }
}
