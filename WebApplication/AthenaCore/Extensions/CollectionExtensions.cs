using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication.AthenaCore.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddAll<TK,TV>(this Dictionary<TK, TV> dictionary, Dictionary<TK, TV> other,
            Action<KeyValuePair<TK, TV>> alreadyContainsKeyCallback = null)
        {
            foreach (var pair in other)
            {
                if (!dictionary.TryAdd(pair.Key, pair.Value))
                {
                    alreadyContainsKeyCallback?.Invoke(pair);
                }
            }
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action.Invoke(item);
            }

            return source;
        }
        
        public static IEnumerable<T> ForEachNotLast<T>(this IEnumerable<T> source, Action<T, bool> action)
        {
            int index = 0;
            foreach (var item in source)
            {
                bool notLast = index != source.Count() - 1;
                action.Invoke(item, notLast);
                index++;
            }

            return source;
        }
        
        public static IEnumerable<T> ForEachIndex<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            int index = 0;
            foreach (var item in source)
            {
                action.Invoke(item, index++);
            }

            return source;
        }
    }
}