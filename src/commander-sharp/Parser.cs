using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jaja.Commander
{
  /// <summary>
  /// Utility class that exposes generic parse method
  /// </summary>
  internal static class Parser
  {
    internal static T Parse<T>(string s)
    {
      return (T)Convert.ChangeType(s, typeof (T));
    }

    /// <summary>
    /// Find the duplicates in a list
    /// </summary>
    internal static IGrouping<TKey, TSource> FindDups<TSource, TKey>(this IEnumerable<TSource> list, Func<TSource, TKey> keySelector) => list
      .GroupBy(keySelector)
      .FirstOrDefault();

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