using System.Linq;
using Xunit;
using System.Collections.Generic;

namespace Jaja.Commander.Test
{
  public class SubCommands
  {

    [Fact]
    public void SimpleSubCommand()
    {
      var program = Commander.New(new {
        Chdir = new Opt<string>('C', desc: "change the working directory"),
        Config = new Opt<string>(desc: "set config path. defaults to ./deploy.conf"),
        NoTests = new Opt('T', desc: "ignore test hook")
      });

      string mode = "", host = "", to = "";

      program.Command("setup", "run setup commands for all envs", new
      {
          SetupMode = new Opt<string>(desc: "which setup mode to use"),
          Host = new Opt<string>(desc: "which host to use")
      }, c => {
        host = c.Options.Host.Value;
        mode = c.Options.SetupMode.Value;
        to = c.Args.First();
      });

      program.Parse(new []{"setup", "-s", "setset", "--host", "hosthost", "location"});
      Assert.Equal("setset", mode);
      Assert.Equal("hosthost", host);
      Assert.Equal("location", to);
    }
  }
}
