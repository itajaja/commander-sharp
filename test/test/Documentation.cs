using Xunit;

namespace Jaja.Commander.Test
{
  public class Documentation
  {
    [Fact]
    public void OutputArgumentDocumentation()
    {
      var options = new {
        NoCheese = new Opt('c', "no-cheese", "Don't use any cheese"),
        Pepperoni = new Opt(desc: "Add Pepperoni"),
        Size = new Opt<string>(desc: "Choose a size for the pizza", isOptional: false),
        Tip = new Opt<int>(desc: "A tip for the delivery guy")
      };
      var program = Commander.New("pizza-cli", options, "Order a pizza through command line");
      var help = program.Help().Split('\n');
      Assert.Equal("pizza-cli", help[0]);
//      Assert.Equal("v0.1.0", help[1]);
      Assert.Equal("Order a pizza through command line", help[2]);
      Assert.Equal("", help[3]);
      Assert.Equal("Options:", help[4]);
      Assert.Equal("   -c, --no-cheese     Don't use any cheese", help[5]);
      Assert.Equal("   -p, --pepperoni     Add Pepperoni", help[6]);
      Assert.Equal("   -s, --size          Choose a size for the pizza", help[7]);
      Assert.Equal("   -t, --tip           A tip for the delivery guy", help[8]);
      Assert.Equal(10, help.Length);
    }

    [Fact]
    public void OutputCommands()
    {
      var program = Commander.New("git", new { }, "the stupid content tracker")
        .Command("init", "Create an empty git repository or reinitialize an existing one", new { }, _ => { })
        .Command("add", "Add new or modified files to the staging area", new { }, _ => { })
        .Command("rm", "Remove files from the working directory and staging area", new { }, _ => { })
        .Command("mv", "Move or rename a file, a directory, or a symlink", new { }, _ => { })
        .Command("status", "Show the status of the working directory and staging area", new { }, _ => { })
        .Command("commit", "Record changes to the repository", new { }, _ => { });

      var help = program.Help().Split('\n');
      Assert.Equal("git", help[0]);
      //      Assert.Equal("v0.1.0", help[1]);
      Assert.Equal("the stupid content tracker", help[2]);
      Assert.Equal("", help[3]);
      Assert.Equal("Options:", help[4]);
      Assert.Equal("   -h, --help     prints this help message", help[5]);
      Assert.Equal("", help[6]);
      Assert.Equal("Commands:", help[7]);
      Assert.Equal("   init       Create an empty git repository or reinitialize an existing one", help[8]);
      Assert.Equal("   add        Add new or modified files to the staging area", help[9]);
      Assert.Equal("   rm         Remove files from the working directory and staging area", help[10]);
      Assert.Equal("   mv         Move or rename a file, a directory, or a symlink", help[11]);
      Assert.Equal("   status     Show the status of the working directory and staging area", help[12]);
      Assert.Equal("   commit     Record changes to the repository", help[13]);
      Assert.Equal(14, help.Length);
    }

    [Fact]
    public void ExecHelp()
    {
      var options = new
      {
        NoCheese = new Opt('c', "no-cheese", "Don't use any cheese"),
        Pepperoni = new Opt(desc: "Add Pepperoni"),
        Size = new Opt<string>(desc: "Choose a size for the pizza", isOptional: false),
        Tip = new Opt<int>(desc: "A tip for the delivery guy")
      };
      var program = Commander.New("pizza-cli", options, "Order a pizza through command line");
      string help = null;
      // mock the print method
      program.WriteToConsole = s => help = s;

      var args = program.Parse(new[] { "-h" });
      Assert.Null(args);
      Assert.Contains("pizza-cli", help);
    }

    [Fact]
    public void OverwriteHelp()
    {
      var options = new
      {
        Hello = new Opt(),
      };
      var program = Commander.New("hello", options);

      string help = null;
      // mock the print method
      program.WriteToConsole = s => help = s;

      var args = program.Parse(new[] { "-h" });
      Assert.NotNull(args);
      Assert.Null(help);
    }
  }
}
