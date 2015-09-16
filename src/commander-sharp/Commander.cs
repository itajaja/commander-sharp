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
    private const string HelpKey = "___$$help$$___";
    private const string VersionKey = "___$$version$$___";
    private const int helpIndentation = 3;
    private const int helpSpace = 5;

    private IDictionary<string, Opt> OptionsDic { get; }

    // contains the callback to call for a specific sub-command
    private IDictionary<string, SubCommand> Subcommands { get; } = new Dictionary<string, SubCommand>();

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
      
      // add help
      if(GetProp(ArgType.ShortOpt, "-h").Value == null && GetProp(ArgType.LongOpt, "--help").Value == null)
        OptionsDic.Add(HelpKey, new Opt('h', "help", "prints this help message"));

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
        Subcommands[args[0]].On(args.Skip(1).ToArray());
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
              SetArgument(prop.Key);
            else
            {
              i++;
              if(parsedArgs.Count == i || parsedArgs[i].type != ArgType.Argument)
                throw new CommanderException($"option {prop.Value.LongName} must have an argument");
              SetArgument(prop.Key, parsedArgs[i].val);
            }
            break;
          case ArgType.ShortOpts:
            foreach (var opt in cur.val.Skip(1))
              SetArgument(GetProp(cur.type, "-" + opt).Key);
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

      // check for help
      if (OptionsDic.ContainsKey(HelpKey) && OptionsDic[HelpKey].IsDefined)
      {
        WriteToConsole(Help());
        return null;
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
    public Commander<T> Command<TSub>(string name, string description, TSub options, Action<Arguments<TSub>> action) {
      var command = Commander.New(name, options, description);
      Action<string[]> on = args => action(command.Parse(args));
      Subcommands[name] = new SubCommand
      {
        Description = description,
        On = on
      };
      return this;
    }

    /// <summary>
    /// Outputs the help message for the command
    /// </summary>
    public string Help()
    {
      var help = new List<string>();
      help.Add(Name);
      help.Add("Version Number (TODO)");
      help.Add(Description);
      Action<List<Tuple<string,string>>> addToHelp = list =>
      {
        var maxlenght = list.Max(o => o.Item1.Length + helpSpace);
        list.Select(i => " ".Repeat(helpIndentation) + i.Item1 + " ".Repeat(maxlenght - i.Item1.Length) + i.Item2)
          .ToList()
          .ForEach(help.Add);
      };
      if (OptionsDic.Any())
      {
        help.Add("");
        help.Add("Options:");
        addToHelp(OptionsDic.Values.Select(o => new Tuple<string, string>($"-{o.ShortName}, --{o.LongName}", o.Desc)).ToList());
      }
      if (Subcommands.Any())
      {
        help.Add("");
        help.Add("Commands:");
        addToHelp(Subcommands.Select(c => new Tuple<string, string>(c.Key, c.Value?.Description)).ToList());
      }
      return string.Join("\n", help);
    }

    #region private methods

    private void CheckRequiredOptions(){
      var missingRequired = OptionsDic.Values
        .OfType<IArgOpt<object>>()
        .FirstOrDefault(o => !o.IsOptional && !o.IsDefined);
      if(missingRequired != null)
        throw new CommanderException($"Property {missingRequired.LongName} is required");
    }

    private void SetArgument(string argName)
    {
      var arg = OptionsDic[argName];
      var prop = arg.GetType();
      if(prop != typeof(Opt))
        throw new CommanderException($"option {arg.LongName} must have an argument");
      if (arg.IsDefined)
        throw new CommanderException($"option {arg.LongName} defined twice");
      arg.IsDefined = true;
    }

    private void SetArgument(string argName, string value)
    {
      var arg = OptionsDic[argName];
      if (arg.IsDefined)
        throw new CommanderException($"option {arg.LongName} defined twice");
      arg.IsDefined = true;
      var coercion = (Delegate) arg.GetType().GetProperty(nameof(Opt<object>.Coercion)).GetValue(arg);
      arg.GetType().GetProperty(nameof(Opt<object>.Value)).SetValue(arg, coercion.DynamicInvoke(value));
    }

    private KeyValuePair<string, Opt> GetProp(ArgType type, string argVal)
    {
      return type == ArgType.LongOpt
        ? OptionsDic.SingleOrDefault(a => a.Value.LongName == argVal.Substring(2))
        : OptionsDic.SingleOrDefault(a => a.Value.ShortName == argVal[1]);
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

    /// <summary>
    /// Gets or sets the method used to print the messages. Defaults to `Console.WriteLine`
    /// </summary>
    public Action<string> WriteToConsole { get; set; } = Console.WriteLine;
  }

  internal enum ArgType
  {
    LongOpt,
    ShortOpt,
    ShortOpts,
    Argument
  }

  internal class SubCommand
  {
    public string Description { get; set; }

    public Action<string[]> On { get; set; }
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
