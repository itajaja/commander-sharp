using Xunit;

namespace Jaja.Commander.Test
{
  public class Tests
  {
    [Fact]
    public void FailDuplicateLongArguments()
    {

      var c = new Commander()
        .Argument('a', "first command")
        .Argument('a', "second command");

      Assert.Throws<CommanderException>(() => c.Parse(new string[0]));
    }

    [Fact]
    public void FailDuplicateShortArguments()
    {

      var c = new Commander()
        .Argument('a', "first command")
        .Argument('b', "first command");

      Assert.Throws<CommanderException>(() => c.Parse(new string[0]));
    }
  }
}
