using System;
using System.Windows;
using System.Windows.Input;
using TourManagementSystem.ViewModels;

namespace TourManagementSystem.Views
{
    public partial class LoginWindow : Window
    {
        private LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();

            // Initialize ViewModel (this should be injected via DI in a real application)
            var context = new Models.TourManagementContext();
            var authService = new Services.AuthService(context);
            _viewModel = new LoginViewModel(authService);

            DataContext = _viewModel;
            _viewModel.LoginSuccessful += OnLoginSuccessful;


        }

        private void OnLoginSuccessful(object? sender, Models.User user)
        {
            try
            {
                // Navigate to main window based on user role
                Window mainWindow;
                switch (user.Role.ToLower())
                {
                    case "admin":
                        mainWindow = new AdminMainWindow(user);
                        break;
                    case "staff":
                        mainWindow = new StaffMainWindow(user);
                        break;
                    case "customer":
                        mainWindow = new CustomerMainWindow(user);
                        break;
                    default:
                        MessageBox.Show($"Unknown user role: {user.Role}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                // Set as main window and show
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();

                // Log the login attempt after successful navigation
                try
                {
                    var context = new Models.TourManagementContext();
                    var activityLogService = new Services.ActivityLogService(context);
                    activityLogService.LogActivity(user.UserId, "LOGIN", "User", user.UserId,
                        $"User '{user.Username}' logged in successfully");
                }
                catch (Exception logEx)
                {
                    // Log error silently to avoid breaking navigation
                    System.Diagnostics.Debug.WriteLine($"Logging error: {logEx.Message}");
                }

                // Close this window
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during navigation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ForgotPasswordText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var forgotPasswordWindow = new ForgotPasswordWindow();
            forgotPasswordWindow.ShowDialog();
        }



        protected override void OnSourceInitialized(System.EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Bind password box to ViewModel
            PasswordBox.PasswordChanged += (s, e) =>
            {
                if (DataContext is LoginViewModel vm)
                {
                    vm.Password = PasswordBox.Password;
                }
            };
        }
    }
}