using System.Linq;

namespace Jaja.Commander
{
  public static class StringExtensions
  {
    /// <summary>
    /// repeat a string for the specified amount of times
    /// </summary>
    public static string Repeat(this string str, int count) =>
      Enumerable.Repeat(str, count).Aggregate((a, b) => a + b);
  }
}
