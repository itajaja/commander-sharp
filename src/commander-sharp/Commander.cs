using System;
using System.Collections.Generic;
using System.Linq;

namespace Jaja.Commander
{
  public static class Commander
  {
    public static Commander<T> New<T>(string name, T options, string description = "")
    {
      return new Commander<T>(name, options, description);
    }
  }

  public class Commander<T>
  {
    private IDictionary<string, Opt> OptionsDic { get; }

    // contains the callback to call for a specific sub-command
    private IDictionary<string, Action<string[]>> Subcommands { get; } = new Dictionary<string, Action<string[]>>();

    ///<summary>
    /// The list of options for the command
    ///</summary>
    public T Options { get; }

    ///<summary>
    /// A description that appears in the help section for the command
    ///</summary>
    public string Description { get; }

    /// <summary>
    /// The name of the command line application
    /// </summary>
    public string Name { get; }

    internal Commander(string name, T options, string description = "")
    {
      var t = typeof(T);

      var optDic = t.GetProperties()
        .Where(prop => typeof(Opt).IsAssignableFrom(prop.PropertyType))
        .ToDictionary(prop => prop.Name, prop =>
        {
          var opt = (Opt)prop.GetValue(options);

          var shortName = char.ToLower(prop.Name.First());
          // defaults the short name to the first character of the property
          if (opt.ShortName == default(char))
            opt.ShortName = shortName;

          // defaults the long name to the property name (with lowercase first character)
          if (string.IsNullOrEmpty(opt.LongName))
            opt.LongName = string.Concat(shortName, prop.Name.Substring(1));

          return opt;
        });

      OptionsDic = optDic;
      Options = options;
      Description = description;
      Name = name;
      // validte the arguments first
      ValidateOptions();
    }

    /// <summary>
    /// Creates a command line parser out of the parsed values
    /// </summary>
    public Arguments<T> Parse(string[] args)
    {
      Func<string, bool> isShort = a => a.Length == 2 && a[0] == '-' && a[1] != '-';
      Func<string, bool> isLong = a => a.Length >= 2 && a[0] == '-' && a[1] == '-';
      Func<string, bool> isShorts = a => a.Length > 2 && a[0] == '-' && a[1] != '-';
      var parsedArgs = args.Select(a => new
      {
        val = a,
        type = isShort(a) ? ArgType.ShortOpt : isLong(a) ? ArgType.LongOpt : isShorts(a) ? ArgType.ShortOpts : ArgType.Argument
      })
      .ToList();

      var newArgs = new List<string>();

      // check for subcommands
      if(args.Length > 0 && Subcommands.ContainsKey(args[0]))
      {
        Subcommands[args[0]](args.Skip(1).ToArray());
        return null;
      }

      // evaluates the argumets
      for (var i = 0; i < parsedArgs.Count; i++)
      {
        var cur = parsedArgs[i];
        switch (cur.type)
        {
          case ArgType.LongOpt:
          case ArgType.ShortOpt:
            var prop = GetProp(cur.type, cur.val);
            if (prop.Value.GetType() == typeof (Opt))
              SetArgument(Options, prop.Key);
            else
            {
              i++;
              if(parsedArgs.Count == i || parsedArgs[i].type != ArgType.Argument)
                throw new CommanderException($"option {prop.Value.LongName} must have an argument");
              SetArgument(Options, prop.Key, parsedArgs[i].val);
            }
            break;
          case ArgType.ShortOpts:
            foreach (var opt in cur.val.Skip(1))
              SetArgument(Options, GetProp(cur.type, "-" + opt).Key);
            break;
          case ArgType.Argument:
            for (; i < parsedArgs.Count; i++)
            {
              cur = parsedArgs[i];
              if (cur.type != ArgType.Argument)
                throw new CommanderException($"invalid option {cur.val} after variadic arguments");
              newArgs.Add(cur.val);
            }
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      CheckRequiredOptions();

      return new Arguments<T>
      {
        Args = newArgs,
        Options = Options
      };
    }

    /// <summary>
    /// Creates a sub command with specific options
    /// </summary>
    public Commander<TSub> Command<TSub>(string name, string description, TSub options, Action<Arguments<TSub>> action) {
      var command = Commander.New(name, options, description);
      Action<string[]> cb = args => action(command.Parse(args));
      Subcommands[name] = cb;
      return command;
    }

    #region private methods

    private void CheckRequiredOptions(){
      var missingRequired = OptionsDic.Values
        .OfType<IArgOpt<object>>()
        .FirstOrDefault(o => !o.IsOptional && !o.IsDefined);
      if(missingRequired != null)
        throw new CommanderException($"Property {missingRequired.LongName} is required");
    }

    private static void SetArgument(T argObject, string argName)
    {
      var prop = typeof(T).GetProperty(argName);
      var arg = (Opt)prop.GetValue(argObject);
      if(prop.PropertyType != typeof(Opt))
        throw new CommanderException($"option {arg.LongName} must have an argument");
      if (arg.IsDefined)
        throw new CommanderException($"option {arg.LongName} defined twice");
      arg.IsDefined = true;
    }

    private static void SetArgument(T argObject, string argName, string value)
    {
      var prop = typeof(T).GetProperty(argName);
      var arg = (Opt)prop.GetValue(argObject);
      if (arg.IsDefined)
        throw new CommanderException($"option {arg.LongName} defined twice");
      arg.IsDefined = true;
      var coercion = (Delegate) arg.GetType().GetProperty(nameof(Opt<object>.Coercion)).GetValue(arg);
      arg.GetType().GetProperty(nameof(Opt<object>.Value)).SetValue(arg, coercion.DynamicInvoke(value));
    }

    private KeyValuePair<string, Opt> GetProp(ArgType type, string argVal)
    {
      return type == ArgType.LongOpt
        ? OptionsDic.Single(a => a.Value.LongName == argVal.Substring(2))
        : OptionsDic.Single(a => a.Value.ShortName == argVal[1]);
    }

    /// <summary>
    /// private method to validate the options
    /// </summary>
    private void ValidateOptions()
    {
      var duplicateShort = OptionsDic.Values.FindDups(o => o.ShortName);
      if (duplicateShort != null)
        throw new CommanderException($"Invalid options, there are two options with key {duplicateShort.Key}");

      var duplicateLong = OptionsDic.Values.FindDups(o => o.LongName);
      if (duplicateLong != null)
        throw new CommanderException($"Invalid options, there are two options with key {duplicateLong.Key}");
    }
    #endregion
  }

  internal enum ArgType
  {
    LongOpt,
    ShortOpt,
    ShortOpts,
    Argument
  }

  /// <summary>
  /// Represent the parsed arguments
  /// </summary>
  public class Arguments<T>
  {
    public T Options { get; internal set; }

    public IList<string> Args { get; internal set; }
  }
}
