using KoEServer.World;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace KoEServer.Server
{
    public class GameServer
    {
        // Fields
        private TcpListener _listener;
        private const int Port = 12345; // Port for the server to listen on
        private GameWorld _gameWorld;

        private List<KoEPlayer> _loggedInPlayers = new List<KoEPlayer>(); 

        private readonly string _connectionString = "Data Source=KoEPlayers.db;Version=3;";

        public GameServer(GameWorld gameWorld)
        {
            _gameWorld = gameWorld;
            InitializeDatabase();
            Console.WriteLine("KoEServer created");
        }
        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, Port);
            _listener.Start();
            Console.WriteLine($"KoEServer started on port {Port}");

            // Start listening for connections
            ThreadPool.QueueUserWorkItem(ListenForClients);
        }

        public void Stop()
        {
            _listener.Stop();
            Console.WriteLine("KoEServer stopped");
        }

        private void ListenForClients(object state)
        {
            while (_gameWorld.IsRunning)
            {
                try
                {
                    var client = _listener.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(HandleClient, client);
                }
                catch (SocketException)
                {
                    if (!_gameWorld.IsRunning) break;
                }
            }
        }

        private void HandleClient(object state)
        {
            var client = (TcpClient)state;
            var stream = client.GetStream();
            var buffer = new byte[1024];
            int bytesRead;

            try
            {
                // Read data from the client
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                var request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // Process the request (e.g., login, create kingdom)
                var response = ProcessRequest(request);

                // Send response back to the client
                var responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        private string ProcessRequest(string request)
        {
            // Example request format: "LOGIN|username|password" or "CREATE_KINGDOM|username|kingdomName"
            var parts = request.Split('|');
            if (parts.Length < 2) return "ERROR|Invalid request";

            var command = parts[0];
            var username = parts[1];

            switch (command)
            {
                case "REGISTER":
                    if (parts.Length < 3) return "ERROR|Invalid registration request";
                    var password = parts[2];
                    return RegisterPlayer(username, password);

                case "LOGIN":
                    if (parts.Length < 3) return "ERROR|Invalid login request";
                    password = parts[2];
                    return LoginPlayer(username, password);

                case "LOGOUT":
                    return LogoutPlayer(username);

                //case "CREATE_KINGDOM":
                //    if (parts.Length < 3) return "ERROR|Invalid kingdom creation request";
                //    var kingdomName = parts[2];
                //    return CreateKingdom(username, kingdomName);

                default:
                    return "ERROR|Unknown command";
            }
        }

        private string LoginPlayer(string username, string password)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("SELECT COUNT(*) FROM Players WHERE Username = @username AND Password = @password", connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                var result = (long)command.ExecuteScalar();

                Console.WriteLine($"Login attempt for {username}: {(result > 0 ? "successful" : "failed")}");

                if (result > 0)
                {
                    // Check if the player is already logged in
                    var existingPlayer = _loggedInPlayers.Find(p => p.Username == username);
                    if (existingPlayer != null)
                    {
                        return "ERROR|Player already logged in";
                    }
                    // Create a new KoEPlayer instance and add it to the logged-in players list
                    var player = new KoEPlayer(username, password) { IsLoggedIn = true, IsRegistered = true };
                    _loggedInPlayers.Add(player);
                    return "SUCCESS|Login successful";
                }

                return result > 0 ? "SUCCESS|Login successful" : "ERROR|Invalid username or password";
            }
        }

        private string LogoutPlayer(string username)
        {
            // Find the player in the logged-in players list
            var player = _loggedInPlayers.Find(p => p.Username == username);

            if (player == null)
            {
                return "ERROR|Player is not logged in";
            }

            // Remove the player from the logged-in players list
            _loggedInPlayers.Remove(player);
            player.IsLoggedIn = false;

            Console.WriteLine($"Player {username} logged out successfully.");
            return "SUCCESS|Player logged out successfully";
        }

        private void InitializeDatabase()
        {
            // Check if the database file exists
            string databaseFile = "KoEPlayers.db";
            bool databaseExists = System.IO.File.Exists(databaseFile);

            if (!databaseExists)
            {
                Console.WriteLine("Database file not found. Creating a new database...");
            }

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                if (!databaseExists)
                {
                    // Create Players table
                    var createPlayersTable = new SQLiteCommand(
                        "CREATE TABLE IF NOT EXISTS Players (Username TEXT PRIMARY KEY, Password TEXT NOT NULL)", connection);
                    createPlayersTable.ExecuteNonQuery();

                    //// Create Kingdoms table
                    //var createKingdomsTable = new SQLiteCommand(
                    //    "CREATE TABLE IF NOT EXISTS Kingdoms (Guid TEXT PRIMARY KEY, Username TEXT NOT NULL, Name TEXT NOT NULL, X INTEGER, Y INTEGER, FOREIGN KEY(Username) REFERENCES Players(Username))", connection);
                    //createKingdomsTable.ExecuteNonQuery();

                    Console.WriteLine("Database and tables created successfully.");
                }
                else
                {
                    Console.WriteLine("Database file found. Using the existing database.");
                }
            }
        }

        private string RegisterPlayer(string username, string password)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                // Check if the username already exists
                var checkCommand = new SQLiteCommand("SELECT COUNT(*) FROM Players WHERE Username = @username", connection);
                checkCommand.Parameters.AddWithValue("@username", username);
                var result = (long)checkCommand.ExecuteScalar();

                if (result > 0)
                {
                    return "ERROR|Username already exists";
                }

                // Insert the new player into the database
                var insertCommand = new SQLiteCommand("INSERT INTO Players (Username, Password) VALUES (@username, @password)", connection);
                insertCommand.Parameters.AddWithValue("@username", username);
                insertCommand.Parameters.AddWithValue("@password", password);
                insertCommand.ExecuteNonQuery();

                Console.WriteLine($"Player {username} registered successfully.");   

                return "SUCCESS|Player registered successfully";
            }
        }
    }
}
