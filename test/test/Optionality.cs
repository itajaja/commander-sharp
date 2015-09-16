using Xunit;

namespace Jaja.Commander.Test
{
  public class Optionality
  {
    [Fact]
    public void AllowOptionals()
    {
      var options = new {
        Cheese = new Opt<string>()
      };
      var args = Tests.CreateArgs("", options);
      Assert.Equal(null, args.Options.Cheese.Value);
    }

    [Fact]
    public void RequireRequireds()
    {
      var options = new {
        Cheese = new Opt<string>(isOptional: false)
      };
      Assert.Throws<CommanderException>(() => Tests.CreateArgs("", options));
      var args = Tests.CreateArgs("-c provolone", options);
      Assert.Equal("provolone", args.Options.Cheese.Value);
    }

    [Fact]
    public void OptionalDefaultValue()
    {
      var options = new {
        Cheese = new Opt<string>(defaultValue: "pecorino")
      };
      var args = Tests.CreateArgs("", options);
      Assert.Equal("pecorino", args.Options.Cheese.Value);
    }
  }
}
