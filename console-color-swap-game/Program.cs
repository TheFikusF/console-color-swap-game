using System.CommandLine;
using CCSG.View;

Console.Clear();
var rootCommand = new RootCommand("Sample app for System.CommandLine");
var countOption = new Option<int>(name: "--count", getDefaultValue: () => 14);
var sizeOption = new Option<int>(name: "--size", getDefaultValue: () => 4);
rootCommand.AddOption(countOption);
rootCommand.AddOption(sizeOption);
rootCommand.SetHandler((count, size) => 
{
    var game = new ConsoleView(count, size);
    CCSG.Utils.ConsoleExtensions.ReadInput(game.HandleInput);
}, countOption, sizeOption);

rootCommand.InvokeAsync(args).Wait();