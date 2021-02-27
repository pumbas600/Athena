using System;
using System.Collections.Generic;

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
    }
}