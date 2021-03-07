// <copyright file="DictionaryExtensionMethods.cs">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Tests.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Dictionary Extension Methods
    /// </summary>
    public static class DictionaryExtensionMethods
    {
        /// <summary>
        /// Validates the contents of the dictionaries are equal.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="otherDictionary">The other dictionary.</param>
        /// <returns><c>true</c> if the contents of the dictionary are equal; otherwise <c>false</c>.</returns>
        public static bool ContentEquals<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> otherDictionary)
        {
            return (otherDictionary ?? new Dictionary<TKey, TValue>())
                .OrderBy(kvp => kvp.Key)
                .SequenceEqual((dictionary ?? new Dictionary<TKey, TValue>())
                                   .OrderBy(kvp => kvp.Key));
        }
    }
}
