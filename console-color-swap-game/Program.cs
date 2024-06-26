using CCSG;
using CCSG.Utils;
using CCSG.View;

int v = 14;
if (args.Length > 1 && args[0] == "-v")
{
    v = int.Parse(args[1]);
}

int h = 4;
if (args.Length > 3 && args[0] == "-h")
{
    h = int.Parse(args[1]);
}

Console.Clear();
var game = new ConsoleView(v, h);
ConsoleExtensions.ReadInput(game.HandleInput);