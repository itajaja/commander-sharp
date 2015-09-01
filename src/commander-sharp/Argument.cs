using System;

namespace Jaja.Commander
{
  public class Arg
  {
    public Arg() { }

    public Arg(char shortName = default(char), string longName = "", string desc = "")
    {
      ShortName = shortName;
      LongName = longName;
      Desc = desc;
    }

    public char ShortName { get; internal set; }

    public string LongName { get; internal set; }

    public string Desc { get; internal set; }
  }

  public class Arg<T> : Arg
  {
    public Arg(char shortName = default(char), string longName = "", string desc = "",
      bool isOptional = true, Func<string, T> coercion = null, T defaultValue = default(T))
      : base(shortName, longName, @desc)
    {
      Coercion = coercion ?? (Utils.Parse<T>);
      DefaultValue = defaultValue;
      IsOptional = isOptional;
    }

    public T Value { get; internal set; }

    /// <summary>
    /// The Coercion of the command
    /// </summary>
    public Func<string, T> Coercion { get; internal set; }

    /// <summary>
    /// Determines if the value is optional or not
    /// </summary>
    public bool IsOptional { get; internal set; }

    /// <summary>
    /// The default value of the property
    /// </summary>
    public T DefaultValue { get; internal set; }
  }
}