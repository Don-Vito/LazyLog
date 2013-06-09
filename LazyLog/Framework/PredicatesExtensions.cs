using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyLog.Framework
{
    public static class PredicatesExtensions
    {
        public static Predicate<T> And<T>(IEnumerable<Predicate<T>> predicates)
        {
            return delegate(T item)
            {
                foreach (Predicate<T> predicate in predicates)
                {
                    if (!predicate(item))
                    {
                        return false;
                    }
                }
                return true;
            };
        }
    }
}
