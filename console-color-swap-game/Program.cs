using System.CommandLine;
using CCSG.View;

var rootCommand = new RootCommand("A console implementation of color-swap game.");
var countOption = new Option<int>(name: "--count", getDefaultValue: () => 14, description: "total amount of beakers.");
countOption.AddAlias("-c");
var sizeOption = new Option<int>(name: "--size", getDefaultValue: () => 4, description: "size of the beakers.");
sizeOption.AddAlias("-s");
var bottomUnknwonOption = new Option<bool>(name: "--bottomUnknown", getDefaultValue: () => true, description: "is items under heighest item hidden from view.");
bottomUnknwonOption.AddAlias("-b");

rootCommand.AddOption(countOption);
rootCommand.AddOption(sizeOption);
rootCommand.AddOption(bottomUnknwonOption);

rootCommand.SetHandler((count, size, bottomUnknown) => 
{
    var game = new ConsoleView(Math.Clamp(count, 6, 18), Math.Clamp(size, 3, 6), bottomUnknown);
    CCSG.Utils.ConsoleExtensions.ReadInput(game.HandleInput);
}, countOption, sizeOption, bottomUnknwonOption);

rootCommand.InvokeAsync(args).Wait();