using KoEServer.Util;
using System.Linq;
using System;

namespace KoEServer.World
{
    public class GenerateWorld
    {
        public GameWorld CreateWorld(int aiCount)
        {
            GameWorld gameWorld = new GameWorld();

            // Load existing kingdoms from the database
            gameWorld.LoadKingdomsFromDatabase();

            // If no kingdoms exist, create new AI kingdoms
            if (!gameWorld.Kingdomes.Any())
            {
                Console.WriteLine("No kingdoms found in the database. Creating new AI kingdoms...");
                for (int i = 0; i < aiCount; i++)
                {
                    var kingdom = new Kingdom($"AI Kingdom {i + 1}");
                    gameWorld.AddKingdom(kingdom, isPlayerControlled: false);
                }
            }
            else
            {
                Console.WriteLine("Existing kingdoms loaded from the database.");
            }

            return gameWorld;
        }
    }
}
