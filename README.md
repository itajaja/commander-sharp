# commander-sharp [![Build Status](https://travis-ci.org/itajaja/commander-sharp.svg?branch=master)](https://travis-ci.org/itajaja/commander-sharp) [![NuGet Status](http://img.shields.io/nuget/v/commander-sharp.svg)](https://www.nuget.org/packages/commander-sharp)

Command line options parser inspired by [nodejs' `commander` package](https://www.npmjs.com/package/commander). Commander-sharp offers you the following features:

- Specify optional or mandatory parameters
- Automatic or custom type coercion
- Automatic help generation
- subcommands à la git

## Usage

````cs
var program = Commander.New("my test program", new
{
  Chdir = new Opt<string>('C', desc: "change the working directory"),
  NoTests = new Opt('T', desc: "ignore test hook"),
  Coer = new Opt<int[]>(coercion: (s) => s.Split(',').Select(n => int.Parse(n)).ToArray())
}, "a test cli to show the capabilities of Commander#");

program.Command("setup", "run setup commands for all envs", new
{
    SetupMode = new Opt<string>(desc: "which setup mode to use"),
    Host = new Opt<string>(desc: "which host to use")
}, c => {
  Console.WriteLine($"you used the setup command with {c.Options.SetupMode.Value} and {c.Options.Host.Value}");
});

var p = program.Parse(args);
if (p != null) {
  Console.WriteLine($"c is {p.Options.Coer.Value?.Length} long");
}
````

refer to the builtin documentation for the method signatures and class descriptions.

## build and test

You must have dnvm, dnx and dnu installed. Run `./build.sh` to test the application.
