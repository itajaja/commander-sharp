using System.Collections.Generic;
using System.Linq;

namespace Jaja.Commander
{
  public static class Commander
  {
    public static Commander<T> New<T>(T arguments)
    {
      var t = typeof(T);
      var argumentType = typeof(Arg);

      var args = t.GetProperties()
        .Where(prop => typeof(Arg).IsAssignableFrom(prop.PropertyType))
        .Select(prop =>
        {
          var arg = (Arg) prop.GetValue(arguments);

          var shortName = char.ToLower(prop.Name.First());
          // defaults the short name to the first character of the property
          if (arg.ShortName == default(char))
            arg.ShortName = shortName;

          // defaults the long name to the property name (with lowercase first character)
          if (string.IsNullOrEmpty(arg.LongName))
            arg.LongName = string.Concat(shortName, prop.Name.Substring(1));

          return arg;
        })
        .ToList();

      return new Commander<T>(args);
    }
  }

  public class Commander<T>
  {
    public IList<Arg> Arguments { get; }

    internal Commander(IList<Arg> arguments)
    {
      Arguments = arguments;
      // validte the arguments first
      ValidateArgs();
    }

    /// <summary>
    /// String that describes the usage of the command
    /// </summary>
    public string Usage { get; set; }

    /// <summary>
    /// Creates a command line parser out of the parsed values
    /// </summary>
    /// <param name="args"></param>
    public T Parse(string[] args)
    {
      return default(T);
    }

    #region private methods
    /// <summary>
    /// private method to validate the arguments
    /// </summary>
    private void ValidateArgs()
    {
      var duplicateShort = Arguments.FindDups(o => o.ShortName);
      if (duplicateShort != null)
        throw new CommanderException($"Invalid commands, there are two commands with key {duplicateShort.Key}");

      var duplicateLong = Arguments.FindDups(o => o.LongName);
      if (duplicateLong != null)
        throw new CommanderException($"Invalid commands, there are two commands with key {duplicateLong.Key}");
    }
    #endregion
  }
}
