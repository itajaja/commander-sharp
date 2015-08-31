using System;
using System.Collections.Generic;

namespace Jaja.Commander
{
  public class Commander
  {
    #region private
    private readonly IList<Argument> _options = new List<Argument>();
    #endregion

    /// <summary>
    /// String that describes the usage of the command
    /// </summary>
    public string Usage { get; set; }

    /// <summary>
    /// Adds an option to the command line argument
    /// </summary>
    /// <param name="shortCommand"></param>
    /// <returns></returns>
    public Commander Argument(char shortCommand, string longCommand = "", string description = "")
    {
      _options.Add(new Argument
      {
        LongCommand = longCommand,
        Description = description,
        ShortCommand = shortCommand
      });
      return this;
    }

    /// <summary>
    /// Adds an option to the command line arguments
    /// </summary>
    /// <param name="shortCommand"></param>
    /// <returns></returns>
    public Commander Argument<T>(char shortCommand, string longCommand = "", string description = "",
      Func<string, T> coercion = null, T defaultVal = default(T), bool optional = true)
    {
      var opt = new Argument<T>
      {
        LongCommand = longCommand,
        Description = description,
        ShortCommand = shortCommand,
        Default = defaultVal,
        Optional = optional
      };
      if (coercion != null)
        opt.Coercion = coercion;
      _options.Add(opt);
      return this;
    }

    /// <summary>
    /// Creates a command line parser out of the parsed values
    /// </summary>
    /// <param name="args"></param>
    public void Parse(string[] args)
    {
      // validte the arguments
      ValidateArgs();
    }

    #region private methods

    /// <summary>
    /// private method to validate the arguments
    /// </summary>
    private void ValidateArgs()
    {
      var duplicateShort = _options.FindDups(o => o.ShortCommand);
      if (duplicateShort != null)
        throw new CommanderException($"Invalid commands, there are two commands with key {duplicateShort.Key}");

      var duplicateLong = _options.FindDups(o => o.LongCommand);
      if (duplicateLong != null)
        throw new CommanderException($"Invalid commands, there are two commands with key {duplicateLong.Key}");
    }
    #endregion
  }
}
