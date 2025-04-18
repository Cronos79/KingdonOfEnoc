using System;

namespace KoEServer.World
{
    public class Kingdom
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public bool IsAlive { get; private set; } = true;
        public bool IsPlayerControlled { get; private set; } = false;

        public string Name { get; private set; }
        public int Population { get; private set; }

        // Location properties
        public int X { get; private set; }
        public int Y { get; private set; }

        public Kingdom(string name)
        {
            Name = name;
            Population = 10;
        }

        public Kingdom(string name, int x, int y, int population, bool isPlayerControlled)
        {
            Name = name;
            X = x;
            Y = y;
            Population = 10;
        }

        public void Update()
        {
            //Console.WriteLine($"{Name} Update");
        }

        public void SetPlayerControlled(bool isPlayerControlled)
        {
            IsPlayerControlled = isPlayerControlled;
        }

        public void SetLocation(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void SetPopulation(int population)
        {
            Population = population;
        }

        public void SetAlive(bool isAlive)
        {
            IsAlive = isAlive;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetId(string id)
        {
            Id = id;
        }

        public void SetId(Guid id)
        {
            Id = id.ToString();
        }
    }
}
