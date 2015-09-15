using System;

namespace Jaja.Commander
{
  public class Opt
  {
    public Opt() { }

    public Opt(char shortName = default(char), string longName = "", string desc = "")
    {
      ShortName = shortName;
      LongName = longName;
      Desc = desc;
    }

    public char ShortName { get; internal set; }

    public string LongName { get; internal set; }

    public string Desc { get; internal set; }

    public bool IsDefined { get; internal set; }
  }

  public interface IOpt
  {
    char ShortName { get; }

    string LongName { get; }

    string Desc { get; }

    bool IsDefined { get; }
  }

  public interface IArgOpt<out T> : IOpt
  {
    /// <summary>
    /// Determines if the value is optional or not
    /// </summary>
    bool IsOptional { get; }

    /// <summary>
    /// The Coercion of the command
    /// </summary>
    Func<string, T> Coercion { get; }

    /// <summary>
    /// The default value of the property
    /// </summary>
    T DefaultValue { get; }

    T Value { get; }
  }

  public class Opt<T> : Opt, IArgOpt<T>
  {
    public Opt(char shortName = default(char), string longName = "", string desc = "",
      bool isOptional = true, Func<string, T> coercion = null, T defaultValue = default(T))
      : base(shortName, longName, @desc)
    {
      Coercion = coercion ?? (Utils.Parse<T>);
      DefaultValue = defaultValue;
      IsOptional = isOptional;
      Value = defaultValue;
    }

    public T Value { get; internal set; }

    public bool IsOptional { get; }

    public T DefaultValue { get; }

    /// <summary>
    /// The Coercion of the command
    /// </summary>
    public Func<string, T> Coercion { get; }

  }
}
