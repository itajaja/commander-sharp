using System;

namespace Jaja.Commander
{
  internal class Argument
  {
    public char ShortCommand { get; set; }

    public string LongCommand { get; set; }

    public string Description { get; set; }
  }

  internal class Argument<T> : Argument
  {
    /// <summary>
    /// The Coercion of the command
    /// </summary>
    public Func<string, T> Coercion { get; set; } = Parser.Parse<T>;

    /// <summary>
    /// Determines if the value is optional or not
    /// </summary>
    public bool Optional { get; set; }

    public T Default { get; set; }
  }
}