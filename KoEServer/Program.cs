using System;
using KoEServer.Server;
using KoEServer.World;

namespace KoEServer
{
    internal class Program
    {
        private static GameWorld _gameWorld;
        private static GameServer _gameServer;
        static void Main(string[] args)
        { 
            Console.WriteLine("Starting Kingdom of Enoc");

            // Load Gameworld or create new
            GenerateWorld generateWorld = new GenerateWorld();
            _gameWorld = generateWorld.CreateWorld(8);

            // Create the game server
            _gameServer = new GameServer(_gameWorld);
            _gameServer.Start();

         

            // Start the game world
            _gameWorld.Start();

            Console.WriteLine("Press any key to stop the game...");
            Console.ReadKey();

            _gameWorld.Stop();
            _gameServer.Stop();
        }
    }

   

   

  
}
