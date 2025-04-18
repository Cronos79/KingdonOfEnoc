using KoEClient.Client;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KoEClient
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        GameLogin _gameLogin;
        public bool IsLoggedIn { get; set; } = false;
        public string UserName { get; set; } = string.Empty;

        public MainWindow()
        {
            //_gameLogin.Register("testuser", "testpassword");
            this.InitializeComponent();
            _gameLogin = new GameLogin("127.0.0.1", 12345);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsLoggedIn)
            {
                Logout();
            }
            else
            {
                Login();
            }          
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            UserName = UserNameTextBox.Text;
            string password = PasswordBox.Password;
            string response = _gameLogin.Register(UserName, password);
            UserNameTextBox.Text = string.Empty;
            PasswordBox.Password = string.Empty;
            if (response.Contains("SUCCESS"))
            {
                StatusTextBlock.Text = "Register successes: " + response;
                response = _gameLogin.Login(UserName, password);
                if (response.Contains("SUCCESS"))
                {
                    StatusTextBlock.Text = "Login successes: " + response;
                    IsLoggedIn = true;
                    ShowLoginButton();
                }
                else
                {
                    StatusTextBlock.Text = "Login failed: " + response;
                    IsLoggedIn = false;
                    ShowLoginButton();
                }
               
            }
            else
            {
                StatusTextBlock.Text = "Register failed: " + response;
                IsLoggedIn = false;
                ShowLoginButton();
            }
        }

        private void ShowLoginButton()
        {
            UserNameTextBox.Text = string.Empty;
            PasswordBox.Password = string.Empty;
            if (IsLoggedIn)
            {
                LoginButton.Content = "Log out";
                RegisterButton.Visibility = Visibility.Collapsed;
                UserNameTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                LoginButton.Content = "Login";
                RegisterButton.Visibility = Visibility.Visible;
                UserNameTextBox.Visibility = Visibility.Visible;
                PasswordBox.Visibility = Visibility.Visible;
            }
        }

        private void Logout()
        {
            string response = _gameLogin.Logout(UserName);
            IsLoggedIn = false;
            ShowLoginButton();
            StatusTextBlock.Text = "Logout successes: " + response;
        }

        private void Login()
        {
            UserName = UserNameTextBox.Text;
            string Password = PasswordBox.Password;
            string response = _gameLogin.Login(UserName, Password);
            if (response.Contains("SUCCESS"))
            {
                StatusTextBlock.Text = "Login successes: " + response;
                IsLoggedIn = true;
                ShowLoginButton();
            }
            else
            {
                StatusTextBlock.Text = "Login failed: " + response;
                IsLoggedIn = false;
                ShowLoginButton();
            }
        }
    }
}
