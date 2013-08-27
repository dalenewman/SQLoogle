using System;
using System.Collections.Generic;
using System.Linq;

namespace Sqloogle.Utilities
{
    public static class LinqExtensions
    {
        public static Boolean IsEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }
    }
}
