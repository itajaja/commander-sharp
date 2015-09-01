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
        Foo = new Arg('a'),
        Bar = new Arg('a')
      };

      Assert.Throws<CommanderException>(() => Commander.New(pars));
    }

    [Fact]
    public void FailDuplicateLongArguments()
    {
      var pars = new
      {
        Foo = new Arg<int>(longName: "foo"),
        Bar = new Arg(longName: "foo")
      };

      Assert.Throws<CommanderException>(() => Commander.New(pars));
    }

    [Fact]
    public void GenerateCommander()
    {
      var c = Commander.New(new
      {
        Foo = new Arg<int>(),
        Bar = new Arg(),
        Waka = new Arg('z'),
        Chu = new Arg(longName: "chuchu"),
      });

      var shorts = c.Arguments.Select(a => a.ShortName).ToList();
      new[] { 'f', 'z', 'b', 'c' }
        .ToList()
        .ForEach(a => Assert.Contains(a, shorts));

      var longs = c.Arguments.Select(a => a.LongName).ToList();
      new[] { "foo", "bar", "waka", "chuchu" }
        .ToList()
        .ForEach(a => Assert.Contains(a, longs));

    }
  }
}
