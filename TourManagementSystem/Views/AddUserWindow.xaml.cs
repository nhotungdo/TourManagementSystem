using System;
using System.Text.RegularExpressions;
using System.Windows;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class AddUserWindow : Window
    {
        private readonly UserService _userService;
        private readonly ActivityLogService _activityLogService;

        public AddUserWindow(UserService userService, ActivityLogService activityLogService)
        {
            InitializeComponent();
            _userService = userService;
            _activityLogService = activityLogService;
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    var user = CreateUserFromForm();

                    if (_userService.CreateUser(user, 1)) // 1 is admin ID
                    {
                        // Log the activity
                        _activityLogService.LogActivity(
                            1, // Current user ID (you might want to pass this as parameter)
                            "CREATE_USER",
                            "User",
                            user.UserId,
                            $"Created new user '{user.Username}' with role '{user.Role}'");

                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        // Check which field is causing the issue
                        var context = new TourManagementContext();
                        if (context.Users.Any(u => u.Username.ToLower() == user.Username.ToLower()))
                        {
                            MessageBox.Show($"Username '{user.Username}' already exists. Please choose a different username.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else if (context.Users.Any(u => u.Email.ToLower() == user.Email.ToLower()))
                        {
                            MessageBox.Show($"Email '{user.Email}' is already registered. Please use a different email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            MessageBox.Show("Failed to create user. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateForm()
        {
            // Username validation
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                MessageBox.Show("Username is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                UsernameTextBox.Focus();
                return false;
            }

            if (UsernameTextBox.Text.Length < 3)
            {
                MessageBox.Show("Username must be at least 3 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                UsernameTextBox.Focus();
                return false;
            }

            // Password validation
            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                MessageBox.Show("Password is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordBox.Focus();
                return false;
            }

            if (PasswordBox.Password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordBox.Focus();
                return false;
            }

            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                ConfirmPasswordBox.Focus();
                return false;
            }

            // Full name validation
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                MessageBox.Show("Full name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                FullNameTextBox.Focus();
                return false;
            }

            // Email validation
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Email is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextBox.Focus();
                return false;
            }

            if (!IsValidEmail(EmailTextBox.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextBox.Focus();
                return false;
            }

            // Phone validation (optional but if provided, must be valid)
            if (!string.IsNullOrWhiteSpace(PhoneTextBox.Text) && !IsValidPhone(PhoneTextBox.Text))
            {
                MessageBox.Show("Please enter a valid phone number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PhoneTextBox.Focus();
                return false;
            }

            // Check if username already exists
            var context = new TourManagementContext();
            if (context.Users.Any(u => u.Username.ToLower() == UsernameTextBox.Text.Trim().ToLower()))
            {
                MessageBox.Show($"Username '{UsernameTextBox.Text.Trim()}' already exists. Please choose a different username.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                UsernameTextBox.Focus();
                return false;
            }

            // Check if email already exists
            if (context.Users.Any(u => u.Email.ToLower() == EmailTextBox.Text.Trim().ToLower()))
            {
                MessageBox.Show($"Email '{EmailTextBox.Text.Trim()}' is already registered. Please use a different email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextBox.Focus();
                return false;
            }

            return true;
        }

        private User CreateUserFromForm()
        {
            var user = new User
            {
                Username = UsernameTextBox.Text.Trim(),
                Password = PasswordBox.Password, // Will be hashed in UserService
                FullName = FullNameTextBox.Text.Trim(),
                Email = EmailTextBox.Text.Trim().ToLower(),
                Phone = string.IsNullOrWhiteSpace(PhoneTextBox.Text) ? null : PhoneTextBox.Text.Trim(),
                Role = (RoleComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Customer",
                IsActive = (StatusComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() == "Active",
                Address = string.IsNullOrWhiteSpace(AddressTextBox.Text) ? null : AddressTextBox.Text.Trim(),
                // DateOfBirth = DateOfBirthPicker.SelectedDate, // User model doesn't have DateOfBirth
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            return user;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            try
            {
                var regex = new Regex(@"^[\+]?[0-9\s\-\(\)]{10,}$");
                return regex.IsMatch(phone);
            }
            catch
            {
                return false;
            }
        }
    }
}