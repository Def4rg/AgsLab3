using GraphicApllication;
using System;

using (Game game = new Game(800, 600, "Laba01"))
{
    Console.WriteLine(game.APIVersion.ToString());
    game.Run();
}