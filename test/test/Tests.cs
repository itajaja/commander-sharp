using System.Linq;
using Xunit;

namespace Jaja.Commander.Test
{
  public class Tests
  {
    [Fact]
    public void FailDuplicateShortArguments()
    {
      var pars = new
      {
        Foo = new Opt('a'),
        Bar = new Opt('a')
      };

      Assert.Throws<CommanderException>(() => Commander.New(pars));
    }

    [Fact]
    public void FailDuplicateLongArguments()
    {
      var pars = new
      {
        Foo = new Opt<int>(longName: "foo"),
        Bar = new Opt(longName: "foo")
      };

      Assert.Throws<CommanderException>(() => Commander.New(pars));
    }

    [Fact]
    public void GenerateCommander()
    {
      var c = Commander.New(new
      {
        Foo = new Opt<int>(),
        Bar = new Opt(),
        Waka = new Opt('z'),
        Chu = new Opt(longName: "chuchu"),
      });

      var shorts = c.OptionsDic.Select(a => a.Value.ShortName).ToList();
      new[] { 'f', 'z', 'b', 'c' }
        .ToList()
        .ForEach(a => Assert.Contains(a, shorts));

      var longs = c.OptionsDic.Select(a => a.Value.LongName).ToList();
      new[] { "foo", "bar", "waka", "chuchu" }
        .ToList()
        .ForEach(a => Assert.Contains(a, longs));
    }

    [Fact]
    public void ParseCorrectly()
    {
      var args = CreateArgs("-f 11 -b -c", new
      {
        Foo = new Opt<int>(),
        Bar = new Opt(),
        Waka = new Opt('z'),
        Chu = new Opt(longName: "chuchu"),
      });
      
      Assert.Equal(args.Options.Foo.Value, 11);
      Assert.Equal(args.Options.Bar.IsDefined, true);
      Assert.Equal(args.Options.Waka.IsDefined, false);
      Assert.Equal(args.Options.Chu.IsDefined, true);
    }

    private static Arguments<T> CreateArgs<T>(string inputArgs, T options) =>
      Commander.New(options).Parse(inputArgs.Split(' '));
  }
}
