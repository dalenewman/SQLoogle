#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace Sqloogle.Libs.NLog.Internal
{
    /// <summary>
    ///     LINQ-like helpers (cannot use LINQ because we must work with .NET 2.0 profile).
    /// </summary>
    internal static class EnumerableHelpers
    {
        /// <summary>
        ///     Filters the given enumerable to return only items of the specified type.
        /// </summary>
        /// <typeparam name="T">
        ///     Type of the item.
        /// </typeparam>
        /// <param name="enumerable">
        ///     The enumerable.
        /// </param>
        /// <returns>
        ///     Items of specified type.
        /// </returns>
        public static IEnumerable<T> OfType<T>(this IEnumerable enumerable)
            where T : class
        {
            foreach (var o in enumerable)
            {
                var t = o as T;
                if (t != null)
                {
                    yield return t;
                }
            }
        }

        /// <summary>
        ///     Reverses the specified enumerable.
        /// </summary>
        /// <typeparam name="T">
        ///     Type of enumerable item.
        /// </typeparam>
        /// <param name="enumerable">
        ///     The enumerable.
        /// </param>
        /// <returns>
        ///     Reversed enumerable.
        /// </returns>
        public static IEnumerable<T> Reverse<T>(this IEnumerable<T> enumerable)
            where T : class
        {
            var tmp = new List<T>(enumerable);
            tmp.Reverse();
            return tmp;
        }

        /// <summary>
        ///     Determines is the given predicate is met by any element of the enumerable.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>True if predicate returns true for any element of the collection, false otherwise.</returns>
        public static bool Any<T>(this IEnumerable<T> enumerable, Predicate<T> predicate)
        {
            foreach (var t in enumerable)
            {
                if (predicate(t))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Converts the enumerable to list.
        /// </summary>
        /// <typeparam name="T">Type of the list element.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>List of elements.</returns>
        public static List<T> ToList<T>(this IEnumerable<T> enumerable)
        {
            return new List<T>(enumerable);
        }
    }
}