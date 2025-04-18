using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading;

namespace KoEServer.World
{
    public class GameWorld
    {
        private Timer _gameLoopTimer;
        private int _gameLoopInterval = 60000; // 1 minute in milliseconds
        private bool _isRunning;
        public bool IsRunning => _isRunning;

        private const int _gridSize = 5000; // Size of the grid (5000x5000)
        public List<Kingdom> Kingdomes { get; set; }

        // Calendar property
        public DateTime CurrentDate { get; private set; }

        // Random number generator and occupied locations
        private Random _random = new Random();
        private HashSet<(int, int)> _occupiedLocations = new HashSet<(int, int)>();

        // SQLite database file
        private readonly string _databaseFile = "GameWorld.db";
        private readonly string _connectionString;

        // Constructor
        public GameWorld()
        {
            Console.WriteLine("GameWorld created");
            Kingdomes = new List<Kingdom>();

            // Initialize the calendar to the first day of spring in 1251
            CurrentDate = new DateTime(1251, 3, 1);

            // Initialize SQLite connection string
            _connectionString = $"Data Source={_databaseFile};Version=3;";

            // Initialize the database
            InitializeDatabase();

            LoadGameStateFromDatabase();
        }

        public void LoadKingdomsFromDatabase()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var selectCommand = new SQLiteCommand("SELECT Id, Name, X, Y, Population, IsPlayerControlled FROM Kingdoms", connection);
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Create a new Kingdom instance using a constructor or by setting properties
                        var kingdom = new Kingdom(reader["Name"].ToString(), Convert.ToInt32(reader["X"]), Convert.ToInt32(reader["Y"]), Convert.ToInt32(reader["Population"]), Convert.ToInt32(reader["IsPlayerControlled"]) == 1);

                        // Set the Id property using reflection if it is read-only
                        var idProperty = typeof(Kingdom).GetProperty("Id");
                        if (idProperty != null && idProperty.CanWrite)
                        {
                            idProperty.SetValue(kingdom, reader["Id"].ToString());
                        }

                        Kingdomes.Add(kingdom);
                        Console.WriteLine($"Loaded Kingdom: {kingdom.Name} at ({kingdom.X}, {kingdom.Y})");
                    }
                }
            }
        }

        /*************************** Public methods ***************************/
        public void Start()
        {
            _isRunning = true;
            _gameLoopTimer = new Timer(GameLoop, null, 0, _gameLoopInterval);
            Console.WriteLine("GameWorld started");
        }

        public void AddKingdom(Kingdom kingdom, bool isPlayerControlled)
        {
            AssignRandomLocation(kingdom);
            kingdom.SetPlayerControlled(isPlayerControlled);
            Kingdomes.Add(kingdom);

            // Save the kingdom to the database
            SaveKingdomToDatabase(kingdom);

            string kingdomType = isPlayerControlled ? "Player" : "AI";
            Console.WriteLine($"Added {kingdomType} {kingdom.Name} at location ({kingdom.X}, {kingdom.Y})");
        }

        public void RemoveKingdom(Kingdom kingdom)
        {
            Kingdomes.Remove(kingdom);

            // Remove the kingdom from the database
            RemoveKingdomFromDatabase(kingdom.Id);
        }

        public void Stop()
        {
            Console.WriteLine("GameWorld stopped");
            _isRunning = false;
            _gameLoopTimer.Dispose();
        }

        public void Update()
        {
            Console.WriteLine("GameWorld Update");

            // Advance the calendar by one month
            AdvanceCalendar();

            foreach (var kingdom in Kingdomes)
            {
                kingdom.Update();
                UpdateKingdomInDatabase(kingdom);
            }

            // Update GameState Database
            UpdateGameStateInDatabase();
        }

        public double CalculateDistance(Kingdom kingdom1, Kingdom kingdom2)
        {
            // Calculate the difference in X and Y coordinates
            int deltaX = kingdom1.X - kingdom2.X;
            int deltaY = kingdom1.Y - kingdom2.Y;

            // Calculate the Euclidean distance
            double distanceInGridUnits = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            // Convert grid units to miles (each grid increment is 10 miles)
            double distanceInMiles = distanceInGridUnits * 10;

            return distanceInMiles;
        }

        /*************************** Private methods ***************************/
        private void GameLoop(object state)
        {
            if (!_isRunning)
            {
                return;
            }
            Update();
        }

        private void AdvanceCalendar()
        {
            // Advance the date by one month
            int newMonth = CurrentDate.Month + 1;
            int newYear = CurrentDate.Year;

            if (newMonth > 12)
            {
                newMonth = 1; // Reset to January
                newYear++;    // Increment the year
            }

            CurrentDate = new DateTime(newYear, newMonth, 1); // Set to the first day of the new month
            Console.WriteLine($"Current Date: {CurrentDate:MMMM yyyy}");           
        }

        private void AssignRandomLocation(Kingdom kingdom)
        {
            int x, y;

            // Ensure the location is unique and at least 200 miles (20 grid units) away from all other kingdoms
            bool isValidLocation;
            do
            {
                x = _random.Next(0, _gridSize);
                y = _random.Next(0, _gridSize);

                isValidLocation = true;

                foreach (var existingKingdom in Kingdomes)
                {
                    // Calculate the distance in grid units
                    int deltaX = existingKingdom.X - x;
                    int deltaY = existingKingdom.Y - y;
                    double distanceInGridUnits = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                    // Check if the distance is less than 20 grid units (200 miles)
                    if (distanceInGridUnits < 20)
                    {
                        isValidLocation = false;
                        break;
                    }
                }
            } while (!isValidLocation);

            // Assign the location and mark it as occupied
            kingdom.SetLocation(x, y);
            _occupiedLocations.Add((x, y));
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(_databaseFile))
            {
                Console.WriteLine("Database file not found. Creating a new database...");
            }

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                // Create Kingdoms table
                var createKingdomsTable = new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS Kingdoms (" +
                    "Id TEXT PRIMARY KEY, " +
                    "Name TEXT NOT NULL, " +
                    "X INTEGER, " +
                    "Y INTEGER, " +
                    "Population INTEGER, " +
                    "IsPlayerControlled INTEGER)", connection);
                createKingdomsTable.ExecuteNonQuery();

                // Create GameState table
                var createGameStateTable = new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS GameState (" +
                    "Id INTEGER PRIMARY KEY, " +
                    "CurrentDate TEXT NOT NULL)", connection);
                createGameStateTable.ExecuteNonQuery();

                // Initialize the GameState table with the default date if it doesn't exist
                var checkGameState = new SQLiteCommand("SELECT COUNT(*) FROM GameState", connection);
                var result = (long)checkGameState.ExecuteScalar();
                if (result == 0)
                {
                    var insertDefaultGameState = new SQLiteCommand(
                        "INSERT INTO GameState (Id, CurrentDate) VALUES (1, @currentDate)", connection);
                    insertDefaultGameState.Parameters.AddWithValue("@currentDate", CurrentDate.ToString("yyyy-MM-dd"));
                    insertDefaultGameState.ExecuteNonQuery();
                }

                Console.WriteLine("Database initialized successfully.");
            }
        }

        private void SaveKingdomToDatabase(Kingdom kingdom)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var insertCommand = new SQLiteCommand(
                    "INSERT INTO Kingdoms (Id, Name, X, Y, Population, IsPlayerControlled) " +
                    "VALUES (@id, @name, @x, @y, @population, @isPlayerControlled)", connection);
                insertCommand.Parameters.AddWithValue("@id", kingdom.Id);
                insertCommand.Parameters.AddWithValue("@name", kingdom.Name);
                insertCommand.Parameters.AddWithValue("@x", kingdom.X);
                insertCommand.Parameters.AddWithValue("@y", kingdom.Y);
                insertCommand.Parameters.AddWithValue("@population", kingdom.Population);
                insertCommand.Parameters.AddWithValue("@isPlayerControlled", kingdom.IsPlayerControlled ? 1 : 0);
                insertCommand.ExecuteNonQuery();
            }
        }

        private void LoadGameStateFromDatabase()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var selectCommand = new SQLiteCommand("SELECT CurrentDate FROM GameState WHERE Id = 1", connection);
                var result = selectCommand.ExecuteScalar();

                if (result != null)
                {
                    CurrentDate = DateTime.Parse(result.ToString());
                    Console.WriteLine($"Loaded Current Date: {CurrentDate:MMMM yyyy}");
                }
            }
        }

        private void UpdateGameStateInDatabase()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var updateCommand = new SQLiteCommand(
                    "UPDATE GameState SET CurrentDate = @currentDate WHERE Id = 1", connection);
                updateCommand.Parameters.AddWithValue("@currentDate", CurrentDate.ToString("yyyy-MM-dd"));
                updateCommand.ExecuteNonQuery();
            }
        }

        private void UpdateKingdomInDatabase(Kingdom kingdom)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var updateCommand = new SQLiteCommand(
                    "UPDATE Kingdoms SET X = @x, Y = @y, Population = @population, IsPlayerControlled = @isPlayerControlled " +
                    "WHERE Id = @id", connection);
                updateCommand.Parameters.AddWithValue("@id", kingdom.Id);
                updateCommand.Parameters.AddWithValue("@x", kingdom.X);
                updateCommand.Parameters.AddWithValue("@y", kingdom.Y);
                updateCommand.Parameters.AddWithValue("@population", kingdom.Population);
                updateCommand.Parameters.AddWithValue("@isPlayerControlled", kingdom.IsPlayerControlled ? 1 : 0);
                updateCommand.ExecuteNonQuery();
            }
        }

        private void RemoveKingdomFromDatabase(string kingdomId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var deleteCommand = new SQLiteCommand("DELETE FROM Kingdoms WHERE Id = @id", connection);
                deleteCommand.Parameters.AddWithValue("@id", kingdomId);
                deleteCommand.ExecuteNonQuery();
            }
        }
    }
}
