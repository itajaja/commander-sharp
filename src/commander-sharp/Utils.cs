using System;
using System.Collections.Generic;
using System.Linq;

namespace Jaja.Commander
{
  internal static class Utils
  {
    /// <summary>
    /// Generic parse method
    /// </summary>
    internal static T Parse<T>(string s)
    {
      return (T)Convert.ChangeType(s, typeof (T));
    }

    /// <summary>
    /// Find the first duplicates in a list
    /// </summary>
    internal static IGrouping<TKey, TSource> FindDups<TSource, TKey>(this IEnumerable<TSource> list, Func<TSource, TKey> keySelector) => list
      .GroupBy(keySelector)
      .FirstOrDefault(g => g.Count() > 1);

    /// <summary>
    /// Camelize a string
    /// </summary>
    internal static string ToCamel(this string input) => new string(input
      .Pair()
      .Select(p => p.Item1 == ' ' ? char.ToUpperInvariant(p.Item2) : p.Item1)
      .ToArray());

    /// <summary>
    /// Aggregate a list of values into pairs
    /// </summary>
    internal static IEnumerable<Tuple<T, T>> Pair<T>(this IEnumerable<T> input)
    {
      var i1 = input.ToArray();
      var i2 = i1.Skip(1).Concat(new[] {default(T)});
      return i1.Zip(i2, (a, b) => new Tuple<T, T>(a, b));
    }
  }
}