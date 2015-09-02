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
    public void ParseOptionsCorrectly()
    {
      var args = CreateArgs("-f 11 -b -c 1 2 3", new Opts1());

      Assert.Equal(11, args.Options.Foo.Value);
      Assert.Equal(true, args.Options.Bar.IsDefined);
      Assert.Equal(false, args.Options.Waka.IsDefined);
      Assert.Equal(true, args.Options.Chu.IsDefined);
    }

    [Fact]
    public void ParseArgsCorrectly()
    {
      var args = CreateArgs("-f 11 -b -c 1 2 3", new Opts1());
      Assert.Contains("1", args.Args);
      Assert.Contains("2", args.Args);
      Assert.Contains("3", args.Args);
    }

    [Fact]
    public void ParseShortBundlesCorrectly()
    {
      var args = CreateArgs("-bz", new Opts1());
      Assert.Equal(false, args.Options.Foo.IsDefined);
      Assert.Equal(true, args.Options.Bar.IsDefined);
      Assert.Equal(true, args.Options.Waka.IsDefined);
      Assert.Equal(false, args.Options.Chu.IsDefined);
    }

    [Fact]
    public void ErrorShortBundle()
    {
      Assert.Throws<CommanderException>(() => CreateArgs("-bf", new Opts1()));
    }

    [Fact]
    public void ErrorOptionWithoutValue()
    {
      Assert.Throws<CommanderException>(() => CreateArgs("-z --foo", new Opts1()));
      Assert.Throws<CommanderException>(() => CreateArgs("-foo -z", new Opts1()));
    }

    [Fact]
    public void ErrorDuplicateOption()
    {
      Assert.Throws<CommanderException>(() => CreateArgs("-b -b", new Opts1()));
    }

    [Fact]
    public void ErrorDuplicateOption2()
    {
      Assert.Throws<CommanderException>(() => CreateArgs("-b -bar", new Opts1()));
    }

    [Fact]
    public void ErrorDuplicateOption3()
    {
      Assert.Throws<CommanderException>(() => CreateArgs("-f 1 -foo 1", new Opts1()));
    }

    private static Arguments<T> CreateArgs<T>(string inputArgs, T options) =>
      Commander.New(options).Parse(inputArgs.Split(' '));

    private class Opts1
    {
      public Opt<int> Foo { get; } = new Opt<int>();
      public Opt Bar { get; } = new Opt();
      public Opt Waka { get; } = new Opt('z');
      public Opt Chu { get; } = new Opt(longName: "chuchu");
    }
  }
}
