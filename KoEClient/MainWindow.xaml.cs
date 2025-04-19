using KoEClient.Client;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KoEClient
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly AppState _appState;

        public MainWindow()
        {
            this.InitializeComponent();
            _appState = new AppState();
            ShowLoginButton();
        }

        private void ShowRegistrationForm()
        {
            var registerButton = new Button
            {
                Content = "Register",
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Attach the event handler dynamically
            registerButton.Click += RegisterButton_Click;

            ContentArea.Content = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5),
                Children =
        {
            new TextBlock { Text = "Register a New Account", FontSize = 20, Margin = new Thickness(0, 0, 0, 10) },
            new TextBlock { Text = "Email:" },
            new TextBox { PlaceholderText = "Enter your email", Margin = new Thickness(0, 0, 0, 10) },
            new TextBlock { Text = "Name:" },
            new TextBox { PlaceholderText = "Enter your name", Margin = new Thickness(0, 0, 0, 10) },
            new TextBlock { Text = "Password:" },
            new PasswordBox { PlaceholderText = "Enter your password", Margin = new Thickness(0, 0, 0, 10) },
            registerButton
        }
            };
        }


        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the StackPanel containing the form
            var stackPanel = ContentArea.Content as StackPanel;

            if (stackPanel != null)
            {
                // Find the input fields by their order in the StackPanel
                var emailTextBox = stackPanel.Children[2] as TextBox; // Email TextBox
                var nameTextBox = stackPanel.Children[4] as TextBox;  // Name TextBox
                var passwordBox = stackPanel.Children[6] as PasswordBox; // PasswordBox

                if (emailTextBox != null && nameTextBox != null && passwordBox != null)
                {
                    // Store the input data into variables
                    string email = emailTextBox.Text;
                    string name = nameTextBox.Text;
                    string password = passwordBox.Password;

                    // Example: Display the data in the StatusTextBlock
                    StatusTextBlock.Text = $"Email: {email}, Name: {name}, Password: {password}";

                    // You can now use these variables for further processing, such as registration logic
                    _appState.UserName = name;
                    string response = _appState.GameLogin.Register(name, password);

                    if (response.Contains("SUCCESS"))
                    {
                        StatusTextBlock.Text = "Register successes: " + response;
                        response = _appState.GameLogin.Login(_appState.UserName, password);
                        if (response.Contains("SUCCESS"))
                        {
                            StatusTextBlock.Text = "Login successes: " + response;
                            _appState.IsLoggedIn = true;
                            ShowLoginButton();
                        }
                        else
                        {
                            StatusTextBlock.Text = "Login failed: " + response;
                            _appState.IsLoggedIn = false;
                            ShowLoginButton();
                        }

                    }
                    else
                    {
                        StatusTextBlock.Text = "Register failed: " + response;
                        _appState.IsLoggedIn = false;
                        ShowLoginButton();
                    }
                }
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (_appState.IsLoggedIn)
            {
                Logout();
            }
            else
            {
                Login();
            }          
        }

        private void SignupButton_Click(object sender, RoutedEventArgs e)
        {
            ShowRegistrationForm();      
        }

        private void ShowLoginButton()
        {
            UserNameTextBox.Text = string.Empty;
            PasswordBox.Password = string.Empty;
            LoginPanel.Visibility = _appState.IsLoggedIn ? Visibility.Collapsed : Visibility.Visible;
            RightInfoBar.Visibility = _appState.IsLoggedIn ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Logout()
        {
            string response = _appState.GameLogin.Logout(_appState.UserName);
            _appState.IsLoggedIn = false;
            ShowLoginButton();
            StatusTextBlock.Text = "Logout successes: " + response;
        }

        private void Login()
        {
            _appState.UserName = UserNameTextBox.Text;
            string Password = PasswordBox.Password;
            string response = _appState.GameLogin.Login(_appState.UserName, Password);
            if (response.Contains("SUCCESS"))
            {
                StatusTextBlock.Text = "Login successes: " + response;
                _appState.IsLoggedIn = true;
                ShowLoginButton();
            }
            else
            {
                StatusTextBlock.Text = "Login failed: " + response;
                _appState.IsLoggedIn = false;
                ShowLoginButton();
            }
        }
    }
}
