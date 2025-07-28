using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class EditUserWindow : Window
    {
        private readonly User _user;
        private readonly UserService _userService;
        private readonly ActivityLogService _activityLogService;

        public EditUserWindow(User user, UserService userService, ActivityLogService activityLogService)
        {
            InitializeComponent();
            _user = user;
            _userService = userService;
            _activityLogService = activityLogService;

            LoadUserData();
        }

        private void LoadUserData()
        {
            // Set header info
            UserInfoText.Text = $"Editing user: {_user.Username} (ID: {_user.UserId})";

            // Load form data
            UsernameTextBox.Text = _user.Username;
            FullNameTextBox.Text = _user.FullName;
            EmailTextBox.Text = _user.Email;
            PhoneTextBox.Text = _user.Phone ?? "";
            AddressTextBox.Text = _user.Address ?? "";
            // DateOfBirthPicker.SelectedDate = _user.DateOfBirth; // User model doesn't have DateOfBirth
            CreatedAtTextBox.Text = _user.CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";

            // Set role
            foreach (ComboBoxItem item in RoleComboBox.Items)
            {
                if (item.Content.ToString() == _user.Role)
                {
                    RoleComboBox.SelectedItem = item;
                    break;
                }
            }

            // Set status
            foreach (ComboBoxItem item in StatusComboBox.Items)
            {
                if ((item.Content.ToString() == "Active" && _user.IsActive == true) ||
                    (item.Content.ToString() == "Inactive" && _user.IsActive == false))
                {
                    StatusComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void UpdateUserButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    UpdateUserFromForm();

                    if (_userService.UpdateUser(_user, 1)) // 1 is admin ID
                    {
                        // Log the activity
                        _activityLogService.LogActivity(
                            1, // Current user ID (you might want to pass this as parameter)
                            "UPDATE_USER",
                            "User",
                            _user.UserId,
                            $"Updated user '{_user.Username}'");

                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update user. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateForm()
        {
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

            return true;
        }

        private void UpdateUserFromForm()
        {
            _user.FullName = FullNameTextBox.Text.Trim();
            _user.Email = EmailTextBox.Text.Trim().ToLower();
            _user.Phone = string.IsNullOrWhiteSpace(PhoneTextBox.Text) ? null : PhoneTextBox.Text.Trim();
            _user.Role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? _user.Role;
            _user.IsActive = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() == "Active";
            _user.Address = string.IsNullOrWhiteSpace(AddressTextBox.Text) ? null : AddressTextBox.Text.Trim();
            // _user.DateOfBirth = DateOfBirthPicker.SelectedDate; // User model doesn't have DateOfBirth
            _user.UpdatedAt = DateTime.Now;
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