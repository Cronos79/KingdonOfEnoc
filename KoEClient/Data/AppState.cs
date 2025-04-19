using KoEClient.Client;

namespace KoEClient
{
    public class AppState
    {
        public string UserName { get; set; } = string.Empty;
        public bool IsLoggedIn { get; set; } = false;
        public GameLogin GameLogin { get; set; } = new GameLogin("127.0.0.1", 12345);
        public PlayerState PlayerState { get; set; } = new PlayerState();
    }
}
