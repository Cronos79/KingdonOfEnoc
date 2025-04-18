using System;
using System.Net.Sockets;
using System.Text;

namespace KoEClient.Client
{
    public class GameLogin
    {
        private readonly string _serverAddress;
        private readonly int _serverPort;

        public GameLogin(string serverAddress, int serverPort)
        {
            _serverAddress = serverAddress;
            _serverPort = serverPort;
        }

        public string Login(string username, string password)
        {
            // Format the request for the server
            string request = $"LOGIN|{username}|{password}";
            return SendRequestToServer(request);
        }

        public string Logout(string username)
        {
            // Format the request for the server
            string request = $"LOGOUT|{username}";
            return SendRequestToServer(request);
        }

        public string Register(string username, string password)
        {
            // Format the request for the server
            string request = $"REGISTER|{username}|{password}";
            return SendRequestToServer(request);
        }

        private string SendRequestToServer(string request)
        {
            try
            {
                using (var client = new TcpClient(_serverAddress, _serverPort))
                using (var stream = client.GetStream())
                {
                    // Send the request to the server
                    byte[] requestBytes = Encoding.UTF8.GetBytes(request);
                    stream.Write(requestBytes, 0, requestBytes.Length);

                    // Read the response from the server
                    byte[] responseBytes = new byte[1024];
                    int bytesRead = stream.Read(responseBytes, 0, responseBytes.Length);
                    string response = Encoding.UTF8.GetString(responseBytes, 0, bytesRead);

                    return response;
                }
            }
            catch (Exception ex)
            {
                return $"ERROR|{ex.Message}";
            }
        }
    }
}

