using System;

namespace KoEServer.Server
{
    public class KoEPlayer
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool IsRegistered { get; set; }

        public Guid KingdomID { get; set; } = Guid.Empty;

        public KoEPlayer(string username, string password)
        {
            Username = username;
            Password = password;
            IsLoggedIn = false;
            IsRegistered = false;
        }
        public void Login()
        {
            // Logic to log in the player
            IsLoggedIn = true;
        }
        public void Register()
        {
            // Logic to register the player
            IsRegistered = true;
        }
    }
}
