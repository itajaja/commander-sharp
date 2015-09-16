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
      Assert.Equal(9, help.Length);
    }
  }
}
