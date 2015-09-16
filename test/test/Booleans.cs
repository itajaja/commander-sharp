using Xunit;

namespace Jaja.Commander.Test
{
  public class Booleans
  {
    [Fact]
    public void Longs()
    {
      var program = Commander.New("pizza", new {
        Pepper = new Opt(desc: "add pepper"),
        NoCheese = new Opt('c', desc: "remove cheese")
      });
      var args = program.Parse(new [] {"--pepper"});
      Assert.Equal(true, args.Options.Pepper.IsDefined);
      Assert.Equal(false, args.Options.NoCheese.IsDefined);
    }

    [Fact]
    public void Shorts()
    {
      var program = Commander.New("pizza", new {
        Pepper = new Opt(desc: "add pepper"),
        NoCheese = new Opt('c', desc: "remove cheese")
      });
      var args = program.Parse(new []{"-c", "-p"});
      Assert.Equal(true, args.Options.Pepper.IsDefined);
      Assert.Equal(true, args.Options.NoCheese.IsDefined);
    }

    [Fact]
    public void ShortCombined()
    {
      var program = Commander.New("pizza", new {
        Pepper = new Opt(desc: "add pepper"),
        NoCheese = new Opt('c', desc: "remove cheese")
      });
      var args = program.Parse(new []{"-pc"});
      Assert.Equal(true, args.Options.Pepper.IsDefined);
      Assert.Equal(true, args.Options.NoCheese.IsDefined);
    }
  }
}
